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
    [Register ("ItemDetailViewController")]
    partial class ItemDetailViewController
    {
        [Outlet]
        UIKit.UIButton BuyButton { get; set; }


        [Outlet]
        UIKit.UILabel ItemDescriptionLabel { get; set; }


        [Outlet]
        UIKit.UIImageView ItemImageView { get; set; }


        [Outlet]
        UIKit.UILabel ItemPriceLabel { get; set; }


        [Outlet]
        UIKit.UILabel ItemTitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BuyButton != null) {
                BuyButton.Dispose ();
                BuyButton = null;
            }

            if (ItemDescriptionLabel != null) {
                ItemDescriptionLabel.Dispose ();
                ItemDescriptionLabel = null;
            }

            if (ItemImageView != null) {
                ItemImageView.Dispose ();
                ItemImageView = null;
            }

            if (ItemPriceLabel != null) {
                ItemPriceLabel.Dispose ();
                ItemPriceLabel = null;
            }

            if (ItemTitleLabel != null) {
                ItemTitleLabel.Dispose ();
                ItemTitleLabel = null;
            }
        }
    }
}