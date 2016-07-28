// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Shopping.DemoApp.iOS.Controllers
{
    [Register ("RatingStreamingViewController")]
    partial class RatingStreamingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView CameraStreamingView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TakePhotoButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CameraStreamingView != null) {
                CameraStreamingView.Dispose ();
                CameraStreamingView = null;
            }

            if (TakePhotoButton != null) {
                TakePhotoButton.Dispose ();
                TakePhotoButton = null;
            }
        }
    }
}