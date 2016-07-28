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

namespace Shopping.DemoApp.iOS
{
    [Register ("SaleItemViewCell")]
    partial class SaleItemViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ItemDescriptionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ItemImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ItemNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ItemPriceLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ItemDescriptionLabel != null) {
                ItemDescriptionLabel.Dispose ();
                ItemDescriptionLabel = null;
            }

            if (ItemImageView != null) {
                ItemImageView.Dispose ();
                ItemImageView = null;
            }

            if (ItemNameLabel != null) {
                ItemNameLabel.Dispose ();
                ItemNameLabel = null;
            }

            if (ItemPriceLabel != null) {
                ItemPriceLabel.Dispose ();
                ItemPriceLabel = null;
            }
        }
    }
}