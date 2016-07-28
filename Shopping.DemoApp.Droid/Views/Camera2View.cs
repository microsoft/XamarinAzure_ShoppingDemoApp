using System;
using System.Collections.Generic;

using Android.OS;
using Android.App;
using Android.Util;
using Android.Media;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;

using Java.IO;
using Java.Nio;
using Java.Lang;

using Math = System.Math;
using Class = Java.Lang.Class;
using Object = Java.Lang.Object;
using Console = System.Console;
using CameraError = Android.Hardware.Camera2.CameraError;
using System.Threading.Tasks;
using System.Linq;

namespace XamarinUtils.Droid
{
    public class Camera2View : FrameLayout
    {
        bool OpeningCamera;

        float DesiredAspectRatio;

        CameraDevice ActiveCameraDevice;
        CameraCaptureSession ActiveCaptureSession;

        public bool BackCameraAvailable;
        string backCameraId;
        Size backCameraSize;
        Size[] backCameraSizes;

        public bool FrontCameraAvailable;
        string frontCameraId;
        Size frontCameraSize;
        Size[] frontCameraSizes;

        TextureView textureView;
        CaptureRequest.Builder previewBuilder;

        CameraStateCallback stateCallback;
        CameraSurfaceTextureListener surfaceTextureListener;

        Size previewSize;

        static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();

        public Camera2View(Activity context) : base(context)
        {
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);

            stateCallback = new CameraStateCallback(this);
            surfaceTextureListener = new CameraSurfaceTextureListener(this);

            textureView = new TextureView(context);
            textureView.SurfaceTextureListener = surfaceTextureListener;

            AddView(textureView);

            GetCameraIds();

            if (backCameraId != null)
            {
                GetBackSizes();
            }

            if (frontCameraId != null)
            {
                GetFrontSizes();
            }

            PrintCameraAvailabilityInfo();
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            if (ActiveCameraDevice == null || ActiveCameraDevice.Id == null)
            {
                return;
            }

            base.OnLayout(changed, left, top, right, bottom);

            float width = right - left;
            float height = bottom - top;

            DesiredAspectRatio = width > height ? width / height : height / width;

            GetBackSize();
            GetFrontSize();

            float actualAspectRatio = 0;

            if (ActiveCameraDevice.Id == backCameraId)
            {
                actualAspectRatio = (float)backCameraSize.Width / (float)backCameraSize.Height;
                previewSize = backCameraSize;
            }
            else if (ActiveCameraDevice.Id == frontCameraId)
            {
                actualAspectRatio = (float)frontCameraSize.Width / (float)frontCameraSize.Height;
                previewSize = frontCameraSize;
            }

            float textureWidth;
            float textureHeight;

            if (width > height)
            {
                textureHeight = height;
                textureWidth = textureHeight * actualAspectRatio;
            }
            else
            {
                textureWidth = width;
                textureHeight = textureWidth * actualAspectRatio;
            }

            textureView.Layout
                (
                (int)((width - textureWidth) / 2f),
                (int)((height - textureHeight) / 2f),
                (int)((width - textureWidth) / 2f + textureWidth),
                (int)((height - textureHeight) / 2f + textureHeight)
            );
        }

        public void OpenBackCamera()
        {
            if (Context == null || (Context as Activity).IsFinishing || OpeningCamera || (ActiveCameraDevice != null && ActiveCameraDevice.Id == backCameraId))
            {
                return;
            }

            try
            {
                if (ActiveCameraDevice != null)
                {
                    ActiveCameraDevice.Close();
                }

                OpeningCamera = true;

                CameraManager manager = (CameraManager)Context.GetSystemService(Context.CameraService);

                manager.OpenCamera(backCameraId, stateCallback, null);

                previewSize = backCameraSize;
            }
            catch (CameraAccessException)
            {
                Toast.MakeText(Context, "Cannot access the camera.", ToastLength.Short).Show();
            }
            catch (NullPointerException)
            {
                Toast.MakeText(Context, "This device doesn't support Camera2 API.", ToastLength.Short).Show();
            }
        }

        public void OpenFrontCamera()
        {
            if (Context == null || (Context as Activity).IsFinishing || OpeningCamera || (ActiveCameraDevice != null && ActiveCameraDevice.Id == frontCameraId))
            {
                return;
            }

            try
            {
                if (ActiveCameraDevice != null)
                {
                    ActiveCameraDevice.Close();
                }

                OpeningCamera = true;

                CameraManager manager = (CameraManager)Context.GetSystemService(Context.CameraService);

                manager.OpenCamera(frontCameraId, stateCallback, null);

                previewSize = frontCameraSize;
            }
            catch (CameraAccessException)
            {
                Toast.MakeText(Context, "Cannot access the camera.", ToastLength.Short).Show();
            }
            catch (NullPointerException)
            {
                Toast.MakeText(Context, "This device doesn't support Camera2 API.", ToastLength.Short).Show();
            }
        }

        public void CloseCamera()
        {
            if (ActiveCameraDevice != null)
            {
                ActiveCameraDevice.Close();
                ActiveCameraDevice = null;
            }
        }

        public Task<byte[]> TakePicture()
        {
            if (Context == null || (Context as Activity).IsFinishing || ActiveCameraDevice == null)
            {
                return Task.FromResult(Enumerable.Empty<byte>().ToArray());
            }

            var file = new File(Context.GetExternalFilesDir(null), "pic.jpg");

            var tcs = new TaskCompletionSource<byte[]>();
            var readerListener = new ImageAvailableListener(tcs, file);

            try
            {
                CameraManager manager = (CameraManager)Context.GetSystemService(Context.CameraService);

                CameraCharacteristics characteristics = manager.GetCameraCharacteristics(ActiveCameraDevice.Id);

                StreamConfigurationMap map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);

                Size[] jpegSizes = map.GetOutputSizes((int)ImageFormatType.Jpeg);

                int width = 640;
                int height = 480;

                if (jpegSizes != null && jpegSizes.Length > 0)
                {
                    width = jpegSizes[0].Width;
                    height = jpegSizes[0].Height;
                }

                ImageReader reader = ImageReader.NewInstance(width, height, ImageFormatType.Jpeg, 1);

                List<Surface> outputSurfaces = new List<Surface> {
                    reader.Surface,
                    new Surface (textureView.SurfaceTexture)
                };

                CaptureRequest.Builder captureBuilder = ActiveCameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
                captureBuilder.AddTarget(reader.Surface);

                SetUpCaptureRequestBuilder(captureBuilder);

                captureBuilder.Set(CaptureRequest.JpegOrientation, new Integer(ORIENTATIONS.Get((int)SurfaceOrientation.Rotation180)));

                HandlerThread thread = new HandlerThread("CameraPicture");
                thread.Start();

                Handler backgroundHandler = new Handler(thread.Looper);

                reader.SetOnImageAvailableListener(readerListener, backgroundHandler);

                ActiveCameraDevice.CreateCaptureSession(outputSurfaces, new CameraCaptureSessionStateCallback(this)
                {
                    OnConfiguredAction = session =>
                    {
                        try
                        {
                            session.Capture(captureBuilder.Build(), null, backgroundHandler);
                        }
                        catch (CameraAccessException ex)
                        {
                            Log.WriteLine(LogPriority.Info, "Capture Session error: ", ex.ToString());
                            tcs.TrySetException(ex);
                        }
                    }
                }, backgroundHandler);
            }
            catch (CameraAccessException ex)
            {
                Log.WriteLine(LogPriority.Info, "Taking picture error: ", ex.StackTrace);
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        public void StartPreview()
        {
            if (ActiveCameraDevice == null || !textureView.IsAvailable || previewSize == null)
            {
                return;
            }

            try
            {
                SurfaceTexture texture = textureView.SurfaceTexture;

                texture.SetDefaultBufferSize(previewSize.Width, previewSize.Height);

                Surface surface = new Surface(texture);

                previewBuilder = ActiveCameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                previewBuilder.AddTarget(surface);

                ActiveCameraDevice.CreateCaptureSession(new List<Surface> { surface },
                    new CameraCaptureSessionStateCallback(this)
                    {
                        OnConfigureFailedAction = session =>
                        {
                            if (Context != null)
                            {
                                Toast.MakeText(Context, "Failed", ToastLength.Short).Show();
                            }
                        },
                        OnConfiguredAction = session =>
                        {
                            ActiveCaptureSession = session;
                            UpdatePreview();
                        }
                    },
                    null);
            }
            catch (CameraAccessException ex)
            {
                Log.WriteLine(LogPriority.Info, "Camera2BasicFragment", ex.StackTrace);
            }
        }

        void GetCameraIds()
        {
            try
            {
                CameraManager manager = (CameraManager)Context.GetSystemService(Context.CameraService);

                string[] cameraIds = manager.GetCameraIdList();

                foreach (string id in cameraIds)
                {
                    CameraCharacteristics characteristics = manager.GetCameraCharacteristics(id);

                    LensFacing lensFacing = (LensFacing)(int)characteristics.Get(CameraCharacteristics.LensFacing);

                    if (lensFacing == LensFacing.Back)
                    {
                        BackCameraAvailable = true;
                        backCameraId = id;
                    }
                    else if (lensFacing == LensFacing.Front)
                    {
                        FrontCameraAvailable = true;
                        frontCameraId = id;
                    }
                }
            }
            catch (CameraAccessException)
            {
                Toast.MakeText(Context, "Cannot access the camera.", ToastLength.Short).Show();
            }
            catch (NullPointerException)
            {
                Toast.MakeText(Context, "This device doesn't support Camera2 API.", ToastLength.Short).Show();
            }
        }

        void GetBackSizes()
        {
            CameraManager manager = (CameraManager)Context.GetSystemService(Context.CameraService);

            CameraCharacteristics characteristics = manager.GetCameraCharacteristics(backCameraId);

            StreamConfigurationMap map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);

            backCameraSizes = map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture)));
        }

        void GetFrontSizes()
        {
            CameraManager manager = (CameraManager)Context.GetSystemService(Context.CameraService);

            CameraCharacteristics characteristics = manager.GetCameraCharacteristics(frontCameraId);

            StreamConfigurationMap map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);

            frontCameraSizes = map.GetOutputSizes(Class.FromType(typeof(SurfaceTexture)));
        }

        void GetBackSize()
        {
            if (backCameraSizes.Length == 0)
            {
                return;
            }

            if (DesiredAspectRatio == 0 || backCameraSizes.Length == 1)
            {
                backCameraSize = backCameraSizes[0];
                return;
            }

            Size bestMatch = null;

            foreach (Size s in backCameraSizes)
            {
                if (((float)s.Width / (float)s.Height - DesiredAspectRatio < 0.01) || ((float)s.Height / (float)s.Width - DesiredAspectRatio < 0.01))
                {
                    if (bestMatch == null)
                    {
                        bestMatch = s;
                    }
                    else
                    {
                        if (s.Width > bestMatch.Width && s.Height > bestMatch.Height)
                        {
                            bestMatch = s;
                        }
                    }
                }
            }

            if (bestMatch == null)
            {
                backCameraSize = backCameraSizes[0];
            }
            else
            {
                backCameraSize = bestMatch;
            }
        }

        void GetFrontSize()
        {
            if (frontCameraSizes.Length == 0)
            {
                return;
            }

            if (DesiredAspectRatio == 0 || frontCameraSizes.Length == 1)
            {
                frontCameraSize = frontCameraSizes[0];
                return;
            }

            Size bestMatch = null;

            foreach (Size s in frontCameraSizes)
            {
                if ((Math.Abs((float)s.Width / (float)s.Height - DesiredAspectRatio) < 0.01) || (Math.Abs((float)s.Height / (float)s.Width - DesiredAspectRatio) < 0.01))
                {
                    if (bestMatch == null)
                    {
                        bestMatch = s;
                    }
                    else
                    {
                        if (s.Width > bestMatch.Width && s.Height > bestMatch.Height)
                        {
                            bestMatch = s;
                        }
                    }
                }
            }

            if (bestMatch == null)
            {
                frontCameraSize = frontCameraSizes[0];
            }
            else
            {
                frontCameraSize = bestMatch;
            }
        }

        void PrintCameraAvailabilityInfo()
        {
            Console.WriteLine("------------------------");
            Console.WriteLine("CAMERA AVAILABILITY INFO");
            Console.WriteLine("------------------------");

            Console.WriteLine("Back camera available: {0}", BackCameraAvailable);

            if (BackCameraAvailable)
            {
                Console.WriteLine("Back camera sizes:");
                foreach (Size s in backCameraSizes)
                {
                    Console.WriteLine("{0} - {1}", s.Width, s.Height);
                }
            }

            Console.WriteLine(" ");

            Console.WriteLine("Front camera available: {0}", FrontCameraAvailable);

            if (FrontCameraAvailable)
            {
                Console.WriteLine("Front camera sizes:");
                foreach (Size s in frontCameraSizes)
                {
                    Console.WriteLine("{0} - {1}", s.Width, s.Height);
                }
            }

            Console.WriteLine("------------------------");
        }

        void UpdatePreview()
        {
            if (ActiveCameraDevice == null || ActiveCaptureSession == null)
            {
                return;
            }

            try
            {
                // The camera preview can be run in a background thread. This is a Handler for the camere preview
                SetUpCaptureRequestBuilder(previewBuilder);
                HandlerThread thread = new HandlerThread("CameraPreview");
                thread.Start();
                Handler backgroundHandler = new Handler(thread.Looper);

                // We start displaying the camera preview
                ActiveCaptureSession.SetRepeatingRequest(previewBuilder.Build(), null, backgroundHandler);
            }
            catch (CameraAccessException ex)
            {
                Log.WriteLine(LogPriority.Info, "Camera2BasicFragment", ex.StackTrace);
            }
            catch (Java.Lang.IllegalStateException ex)
            {
                Log.WriteLine(LogPriority.Info, "Camera2BasicFragment", ex.StackTrace);
            }
        }

        void SetUpCaptureRequestBuilder(CaptureRequest.Builder builder)
        {
            // In this sample, we just let the camera device pick the automatic settings
            builder.Set(CaptureRequest.ControlMode, new Integer((int)ControlMode.Auto));

            if (ActiveCameraDevice.Id == backCameraId)
            {
                builder.Set(CaptureRequest.FlashMode, new Integer((int)FlashMode.Off));
            }
            else if (ActiveCameraDevice.Id == frontCameraId)
            {
                builder.Set(CaptureRequest.FlashMode, new Integer((int)FlashMode.Off));
            }
        }

        void ConfigureTransform(int viewWidth, int viewHeight)
        {
            if (textureView == null || previewSize == null || Context == null)
            {
                return;
            }

            SurfaceOrientation rotation = (Context as Activity).WindowManager.DefaultDisplay.Rotation;

            Matrix matrix = new Matrix();

            if (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270)
            {
                RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
                RectF bufferRect = new RectF(0, 0, previewSize.Width, previewSize.Height);

                bufferRect.Offset(viewRect.CenterX() - bufferRect.CenterX(), viewRect.CenterY() - bufferRect.CenterY());

                float scaleH = (float)viewHeight / previewSize.Width;
                float scaleW = scaleH * 4f / 3f * 4f / 3f;

                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                matrix.PostRotate(90 * ((int)rotation - 2), viewRect.CenterX(), viewRect.CenterY());
                matrix.PostScale(scaleW, scaleH, viewRect.CenterX(), viewRect.CenterY());
            }

            textureView.SetTransform(matrix);
        }

        class CameraStateCallback : CameraDevice.StateCallback
        {
            readonly Camera2View CameraView;

            public CameraStateCallback(Camera2View cameraView)
            {
                CameraView = cameraView;
            }

            public override void OnOpened(CameraDevice camera)
            {
                CameraView.ActiveCameraDevice = camera;

                CameraView.StartPreview();

                CameraView.OpeningCamera = false;
            }

            public override void OnDisconnected(CameraDevice camera)
            {
                camera.Close();

                CameraView.ActiveCameraDevice = null;
                CameraView.OpeningCamera = false;
            }

            public override void OnError(CameraDevice camera, CameraError error)
            {
                camera.Close();

                CameraView.ActiveCameraDevice = null;
                CameraView.OpeningCamera = false;
            }
        }

        class CameraCaptureSessionStateCallback : CameraCaptureSession.StateCallback
        {
            Camera2View CameraView;

            public Action<CameraCaptureSession> OnConfiguredAction;
            public Action<CameraCaptureSession> OnConfigureFailedAction;

            public CameraCaptureSessionStateCallback(Camera2View cameraView)
            {
                CameraView = cameraView;
            }

            public override void OnConfigured(CameraCaptureSession session)
            {
                if (OnConfiguredAction != null)
                {
                    OnConfiguredAction(session);
                }
            }

            public override void OnConfigureFailed(CameraCaptureSession session)
            {
                if (OnConfigureFailedAction != null)
                {
                    OnConfigureFailedAction(session);
                }
            }
        }

        class CameraSurfaceTextureListener : Object, TextureView.ISurfaceTextureListener
        {
            readonly Camera2View CameraView;

            public CameraSurfaceTextureListener(Camera2View cameraView)
            {
                CameraView = cameraView;
            }

            public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
            {
                CameraView.ConfigureTransform(width, height);
                CameraView.StartPreview();
            }

            public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
            {
                return true;
            }

            public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
            {
                CameraView.ConfigureTransform(width, height);
            }

            public void OnSurfaceTextureUpdated(SurfaceTexture surface)
            {
            }
        }

        class ImageAvailableListener : Object, ImageReader.IOnImageAvailableListener
        {
            private readonly TaskCompletionSource<byte[]> tcs;
            private readonly File file;

            public ImageAvailableListener(TaskCompletionSource<byte[]> tcs, File file)
            {
                this.file = file;
                this.tcs = tcs;
            }

            public void OnImageAvailable(ImageReader reader)
            {
                Image image = null;

                try
                {
                    image = reader.AcquireLatestImage();

                    ByteBuffer buffer = image.GetPlanes()[0].Buffer;

                    byte[] bytes = new byte[buffer.Capacity()];

                    buffer.Get(bytes);

                    Save(bytes);

                    this.tcs.TrySetResult(bytes);
                }
                catch (Java.Lang.Exception ex)
                {
                    this.tcs.TrySetException(ex);
                }
                catch (System.Exception ex)
                {
                    this.tcs.TrySetException(ex);
                }
                finally
                {
                    if (image != null)
                    {
                        image.Close();
                    }
                }
            }

            void Save(byte[] bytes)
            {
                OutputStream output = null;

                try
                {
                    if (file != null)
                    {
                        output = new FileOutputStream(file);
                        output.Write(bytes);
                    }
                }
                finally
                {
                    if (output != null)
                    {
                        output.Close();
                    }
                }
            }
        }
    }
}