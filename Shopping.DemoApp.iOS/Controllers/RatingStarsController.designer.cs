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
    [Register ("RatingStarsController")]
    partial class RatingStarsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView FullStarsContainerView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FullStarsContainerView != null) {
                FullStarsContainerView.Dispose ();
                FullStarsContainerView = null;
            }
        }
    }
}