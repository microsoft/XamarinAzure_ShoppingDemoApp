// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Shopping.DemoApp.iOS.Controllers
{
	[Register ("AddSaleItemViewController")]
	partial class AddSaleItemViewController
	{
		[Outlet]
		UIKit.UIButton AddItemButton { get; set; }

		[Outlet]
		UIKit.UIButton AddPhotoButton { get; set; }

		[Outlet]
		UIKit.UIView AddPhotoContainerView { get; set; }

		[Outlet]
		UIKit.UIScrollView ContentScrollView { get; set; }

		[Outlet]
		UIKit.UILabel ItemDescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIView ItemDescriptionLineView { get; set; }

		[Outlet]
		UIKit.UITextView ItemDescriptionTextView { get; set; }

		[Outlet]
		UIKit.UILabel ItemNameLabel { get; set; }

		[Outlet]
		UIKit.UIView ItemNameLineView { get; set; }

		[Outlet]
		UIKit.UITextField ItemNameTextField { get; set; }

		[Outlet]
		UIKit.UIImageView ItemPhotoView { get; set; }

		[Outlet]
		UIKit.UILabel ItemPriceLabel { get; set; }

		[Outlet]
		UIKit.UITextField ItemPriceTextField { get; set; }

		[Outlet]
		UIKit.UIView PhotoAddedContainerView { get; set; }

		[Outlet]
		UIKit.UIButton RemovePhotoButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ItemPhotoView != null) {
				ItemPhotoView.Dispose ();
				ItemPhotoView = null;
			}

			if (AddItemButton != null) {
				AddItemButton.Dispose ();
				AddItemButton = null;
			}

			if (AddPhotoButton != null) {
				AddPhotoButton.Dispose ();
				AddPhotoButton = null;
			}

			if (AddPhotoContainerView != null) {
				AddPhotoContainerView.Dispose ();
				AddPhotoContainerView = null;
			}

			if (ContentScrollView != null) {
				ContentScrollView.Dispose ();
				ContentScrollView = null;
			}

			if (ItemDescriptionLabel != null) {
				ItemDescriptionLabel.Dispose ();
				ItemDescriptionLabel = null;
			}

			if (ItemDescriptionLineView != null) {
				ItemDescriptionLineView.Dispose ();
				ItemDescriptionLineView = null;
			}

			if (ItemDescriptionTextView != null) {
				ItemDescriptionTextView.Dispose ();
				ItemDescriptionTextView = null;
			}

			if (ItemNameLabel != null) {
				ItemNameLabel.Dispose ();
				ItemNameLabel = null;
			}

			if (ItemNameLineView != null) {
				ItemNameLineView.Dispose ();
				ItemNameLineView = null;
			}

			if (ItemNameTextField != null) {
				ItemNameTextField.Dispose ();
				ItemNameTextField = null;
			}

			if (ItemPriceLabel != null) {
				ItemPriceLabel.Dispose ();
				ItemPriceLabel = null;
			}

			if (ItemPriceTextField != null) {
				ItemPriceTextField.Dispose ();
				ItemPriceTextField = null;
			}

			if (PhotoAddedContainerView != null) {
				PhotoAddedContainerView.Dispose ();
				PhotoAddedContainerView = null;
			}

			if (RemovePhotoButton != null) {
				RemovePhotoButton.Dispose ();
				RemovePhotoButton = null;
			}
		}
	}
}
