#pragma warning disable SA1652 // Enable XML documentation output
// <copyright file="MediaCaptureHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Shopping.DemoApp.UWP.Helpers
#pragma warning restore SA1652 // Enable XML documentation output
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Shopping.DemoApp.Helpers;
    using Windows.Devices.Enumeration;
    using Windows.Devices.Sensors;
    using Windows.Foundation;
    using Windows.Graphics.Display;
    using Windows.Graphics.Imaging;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.Storage.FileProperties;
    using Windows.Storage.Streams;
    using Windows.System.Display;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public class MediaCaptureHelper: IDisposable
    {
        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

        private readonly DisplayRequest displayRequest = new DisplayRequest();
        private readonly DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
        private readonly SimpleOrientationSensor orientationSensor = SimpleOrientationSensor.GetDefault();

        private MediaCapture mediaCapture;
        private bool isInitialized;
        private bool isPreviewing;
        private bool externalCamera;
        private bool mirroringPreview;

        private CaptureElement previewControl;

        private DisplayOrientations displayOrientation = DisplayOrientations.Portrait;
        private SimpleOrientation deviceOrientation = SimpleOrientation.NotRotated;

        public async Task InitializeCameraAsync(CaptureElement previewControl)
        {
            this.previewControl = previewControl;

            if (this.mediaCapture == null)
            {
                var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                DeviceInformation cameraDevice =
                    allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null &&
                    x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);

                // If there is no camera on the specified panel, get any camera
                cameraDevice = cameraDevice ?? allVideoDevices.FirstOrDefault();

                if (cameraDevice == null)
                {
                    Dialog.Show("No camera device found.");
                    return;
                }

                // Create MediaCapture and its settings
                this.mediaCapture = new MediaCapture();

                var mediaInitSettings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id};

                // Initialize MediaCapture
                try
                {
                    await this.mediaCapture.InitializeAsync(mediaInitSettings);

                    var resolutions = this.mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo);

                    this.isInitialized = true;
                }
                catch (UnauthorizedAccessException)
                {
                    Dialog.Show("The app was denied access to the camera");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception when initializing MediaCapture with {0}: {1}", cameraDevice.Id, ex.ToString());
                }

                // If initialization succeeded, start the preview
                if (this.isInitialized)
                {
                    // Figure out where the camera is located
                    if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                    {
                        // No information on the location of the camera, assume it's an external camera, not integrated on the device
                        this.externalCamera = true;
                    }
                    else
                    {
                        // Camera is fixed on the device
                        this.externalCamera = false;

                        // Only mirror the preview if the camera is on the front panel
                        this.mirroringPreview = cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front;
                    }

                    await this.StartPreviewAsync();
                }
            }

            this.RegisterOrientationEventHandlers();
        }

        public async Task StartPreviewAsync()
        {
            // Prevent the device from sleeping while the preview is running
            this.displayRequest.RequestActive();

            // Set the preview source in the UI and mirror it if necessary
            this.previewControl.Source = this.mediaCapture;
            this.previewControl.FlowDirection = this.mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // Start the preview
            try
            {
                await this.mediaCapture.StartPreviewAsync();
                this.isPreviewing = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when starting the preview: {0}", ex.ToString());
            }

            // Initialize the preview to the current orientation
            if (this.isPreviewing)
            {
                await this.SetPreviewRotationAsync();
            }
        }

        public async Task StopPreviewAsync(CoreDispatcher dispatcher)
        {
            // Stop the preview
            try
            {   
                this.isPreviewing = false;
                await this.mediaCapture.StopPreviewAsync();                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when stopping the preview: {0}", ex.ToString());
            }

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Cleanup the UI
                this.previewControl.Source = null;

                // Allow the device screen to sleep now that the preview is stopped
                displayRequest.RequestRelease();
            });

        }

        public async Task<InMemoryRandomAccessStream> TakePhotoAsync()
        {
            var memoryStream = new InMemoryRandomAccessStream();
            try
            {
                Debug.WriteLine("Taking photo...");
                await this.mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), memoryStream);
                Debug.WriteLine("Photo taken!");
                memoryStream.Seek(0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when taking a photo: {0}", ex.ToString());
            }

            return memoryStream;
        }

        public async Task<InMemoryRandomAccessStream> RotatePhoto(InMemoryRandomAccessStream notRotated)
        {
            var memoryStream = new InMemoryRandomAccessStream();

            var photoOrientation = ConvertOrientationToPhotoOrientation(this.GetCameraOrientation());
            memoryStream = await ReencodeAndSavePhotoAsync(notRotated, photoOrientation);

            return memoryStream;

        }
        public void RegisterOrientationEventHandlers()
        {
            // If there is an orientation sensor present on the device, register for notifications
            if (this.orientationSensor != null)
            {
                this.orientationSensor.OrientationChanged += this.OrientationSensor_OrientationChanged;
                this.deviceOrientation = this.orientationSensor.GetCurrentOrientation();
            }

            this.displayInformation.OrientationChanged += this.DisplayInformation_OrientationChanged;
            this.displayOrientation = this.displayInformation.CurrentOrientation;
        }

        public void UnregisterOrientationEventHandlers()
        {
            if (this.orientationSensor != null)
            {
                this.orientationSensor.OrientationChanged -= this.OrientationSensor_OrientationChanged;
            }

            this.displayInformation.OrientationChanged -= this.DisplayInformation_OrientationChanged;
        }

        private static async Task<InMemoryRandomAccessStream> ReencodeAndSavePhotoAsync(IRandomAccessStream stream, PhotoOrientation photoOrientation)
        {
            using (var inputStream = stream)
            {
                var decoder = await BitmapDecoder.CreateAsync(inputStream);

                var memoryStream = new InMemoryRandomAccessStream();

                var encoder = await BitmapEncoder.CreateForTranscodingAsync(memoryStream, decoder);

                var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };

                await encoder.BitmapProperties.SetPropertiesAsync(properties);
                await encoder.FlushAsync();

                return memoryStream;
            }
        }

        private static PhotoOrientation ConvertOrientationToPhotoOrientation(SimpleOrientation orientation)
        {
            return PhotoOrientation.Rotate90;
        }

        private static int ConvertDisplayOrientationToDegrees(DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                    return 90;
                case DisplayOrientations.LandscapeFlipped:
                    return 180;
                case DisplayOrientations.PortraitFlipped:
                    return 270;
                case DisplayOrientations.Landscape:
                default:
                    return 0;
            }
        }

        private SimpleOrientation GetCameraOrientation()
        {
            if (this.externalCamera)
            {
                // Cameras that are not attached to the device do not rotate along with it, so apply no rotation
                return SimpleOrientation.NotRotated;
            }

            // If the preview is being mirrored for a front-facing camera, then the rotation should be inverted
            if (this.mirroringPreview)
            {
                // This only affects the 90 and 270 degree cases, because rotating 0 and 180 degrees is the same clockwise and counter-clockwise
                switch (this.deviceOrientation)
                {
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                        return SimpleOrientation.Rotated270DegreesCounterclockwise;
                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        return SimpleOrientation.Rotated90DegreesCounterclockwise;
                }
            }

            return this.deviceOrientation;
        }

        private async Task SetPreviewRotationAsync()
        {
            // Only need to update the orientation if the camera is mounted on the device
            if (this.externalCamera)
            {
                return;
            }

            // Populate orientation variables with the current state
            this.displayOrientation = this.displayInformation.CurrentOrientation;

            // Calculate which way and how far to rotate the preview
            int rotationDegrees = ConvertDisplayOrientationToDegrees(this.displayOrientation);

            // The rotation direction needs to be inverted if the preview is being mirrored
            if (this.mirroringPreview)
            {
                rotationDegrees = (360 - rotationDegrees) % 360;
            }

            // Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering and getting preview frames
            var props = this.mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            props.Properties.Add(RotationKey, rotationDegrees);
            await this.mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
        }

        private void OrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            if (args.Orientation != SimpleOrientation.Faceup && args.Orientation != SimpleOrientation.Facedown)
            {
                this.deviceOrientation = args.Orientation;
            }
        }

        private async void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            this.displayOrientation = sender.CurrentOrientation;

            if (this.isPreviewing)
            {
                await this.SetPreviewRotationAsync();
            }
        }

        public void Dispose()
        {
            if (this.mediaCapture != null)
            {
                this.mediaCapture.Dispose();
                this.mediaCapture = null;
            }

        }
    }
}
