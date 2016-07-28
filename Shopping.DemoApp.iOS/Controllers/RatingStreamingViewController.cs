using Acr.UserDialogs;
using AVFoundation;
using Foundation;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace Shopping.DemoApp.iOS.Controllers
{
    public partial class RatingStreamingViewController : UIViewController
    {
        private AVCaptureSession captureSession;
        private AVCaptureDeviceInput captureDeviceInput;
        private AVCaptureStillImageOutput stillImageOutput;
        private AVCaptureVideoPreviewLayer videoPreviewLayer;
		private bool cameraExists;

        public NSData CaptureData { get; set; }

        public RatingStreamingViewController (IntPtr handle) : base (handle)
        {
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            await AuthorizeCameraUse();
            SetupCameraStream();

            TakePhotoButton.TouchUpInside += TakePhoto;
        }

        private async Task AuthorizeCameraUse()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

            if (authorizationStatus != AVAuthorizationStatus.Authorized)
            {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
            }
        }

        private async void SetupCameraStream()
        {
			// Implementation based on https://blog.xamarin.com/how-to-display-camera-ios-avfoundation/
			captureSession = new AVCaptureSession();

            var viewLayer = CameraStreamingView.Layer;
            videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                Frame = this.View.Frame
            };

            CameraStreamingView.Layer.AddSublayer(videoPreviewLayer);

            AVCaptureDevice[] captureDevices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            var frontDevice = captureDevices.FirstOrDefault(d => d.Position == AVCaptureDevicePosition.Front);

            if (frontDevice != null)
            {
                ConfigureCameraForDevice(frontDevice);
                captureDeviceInput = AVCaptureDeviceInput.FromDevice(frontDevice);
                captureSession.AddInput(captureDeviceInput);

                var dictionary = new NSMutableDictionary();
                dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
                stillImageOutput = new AVCaptureStillImageOutput()
                {
                    OutputSettings = new NSDictionary()
                };

                captureSession.AddOutput(stillImageOutput);
                captureSession.StartRunning();
            }
            else
            {
                await UserDialogs.Instance.AlertAsync("Sorry, no front camera could be detected :-(");
            }

			cameraExists = frontDevice != null;
			TakePhotoButton.Enabled = cameraExists;
        }

        private void ConfigureCameraForDevice(AVCaptureDevice device)
        {
            var error = new NSError();
            if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration();
            }
            else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                device.LockForConfiguration(out error);
                device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                device.UnlockForConfiguration();
            }
            else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
            {
                device.LockForConfiguration(out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                device.UnlockForConfiguration();
            }
        }

        private async void TakePhoto(object sender, EventArgs e)
        {
			if (cameraExists)
			{
				var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
				var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);

				CaptureData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);

				RatingMainViewController parentController = ParentViewController as RatingMainViewController;
				parentController?.ActivateSubmitting();
			}
        }
    }
}