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
    [Register ("RatingProcessingViewController")]
    partial class RatingProcessingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView CaptureImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MessageLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RepeatPhotoButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView StarsView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SubmitRatingButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TopLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CaptureImageView != null) {
                CaptureImageView.Dispose ();
                CaptureImageView = null;
            }

            if (MessageLabel != null) {
                MessageLabel.Dispose ();
                MessageLabel = null;
            }

            if (RepeatPhotoButton != null) {
                RepeatPhotoButton.Dispose ();
                RepeatPhotoButton = null;
            }

            if (StarsView != null) {
                StarsView.Dispose ();
                StarsView = null;
            }

            if (SubmitRatingButton != null) {
                SubmitRatingButton.Dispose ();
                SubmitRatingButton = null;
            }

            if (TopLabel != null) {
                TopLabel.Dispose ();
                TopLabel = null;
            }
        }
    }
}