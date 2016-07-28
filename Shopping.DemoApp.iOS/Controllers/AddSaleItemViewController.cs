using System;
using System.Drawing;
using Acr.UserDialogs;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Shopping.DemoApp.iOS.Extensions;
using Shopping.DemoApp.Models;
using Shopping.DemoApp.Services;
using UIKit;

namespace Shopping.DemoApp.iOS.Controllers
{
	public partial class AddSaleItemViewController : UIViewController
    {
		private readonly string DefaultPriceText = default(decimal).ToString("0.00");

		private NSObject keyboardWillShowNotification;
		private NSObject keyboardWillHideNotification;
		private bool itemPriceTextFieldXPosInitialized;
		private nfloat itemPriceTextFieldXPos;

		private UIView lastResponder;
		private MediaFile photoFile;
		private SaleItem saleItem = new SaleItem();

        public AddSaleItemViewController (IntPtr handle) : base (handle)
		{
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.AddDefaultNavigationTitle();

			UITapGestureRecognizer tapRecognizer = new UITapGestureRecognizer(OnTapGesture);
			ContentScrollView.AddGestureRecognizer(tapRecognizer);

			ItemPriceTextField.Text = DefaultPriceText;
            ItemPriceTextField.EditingDidEnd += ItemPriceTextFieldEditingDidEnd;
			ItemPriceTextField.ShouldChangeCharacters += ItemPriceTextFieldShouldChangeCharacters;

			PhotoAddedContainerView.Hidden = true;
			AddPhotoButton.TouchUpInside += OnAddPhotoButtonTapped;
			RemovePhotoButton.TouchUpInside += OnRemovePhotoButtonTapped;
			AddItemButton.TouchUpInside += OnAddItemButtonTapped;
		}

        public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			keyboardWillShowNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyboardWillShow);
			keyboardWillHideNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyboardWillHide);
			ContentScrollView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			if (keyboardWillShowNotification != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardWillShowNotification);
			}

			if (keyboardWillHideNotification != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardWillHideNotification);
			}
		}

		private async void OnAddPhotoButtonTapped(object sender, EventArgs e)
		{
			MediaFile mediaFile = await CrossMedia.Current.PickPhotoAsync();

			if (mediaFile != null)
			{
				photoFile = mediaFile;
				ItemPhotoView.Image = UIImage.FromFile(photoFile.Path);
				PhotoAddedContainerView.Hidden = false;
			}
		}

		private void OnRemovePhotoButtonTapped(object sender, EventArgs e)
		{
			photoFile = null;
			ItemPhotoView.Image = null;
			PhotoAddedContainerView.Hidden = true;
		}

		private async void OnAddItemButtonTapped(object sender, EventArgs e)
		{
			decimal price;

			if (!decimal.TryParse(ItemPriceTextField.Text, out price))
			{
				await UserDialogs.Instance.AlertAsync("Invalid price");
				return;
			}

			UserDialogs.Instance.ShowLoading("Submitting...");

			try
			{
				saleItem = new SaleItem
				{
					Name = ItemNameTextField.Text,
					Description = ItemDescriptionTextView.Text,
					Price = price
				};

				await SaleItemDataService.Instance.AddItemAsync(saleItem, photoFile.Path);

				UserDialogs.Instance.HideLoading();
				NavigationController.PopViewController(true);
			}
			catch
			{
				UserDialogs.Instance.HideLoading();
				await UserDialogs.Instance.AlertAsync("An error ocurred");
			}
		}

		private void ResizePriceInput()
		{
			ResizePriceInput(ItemPriceTextField.Text);
		}

		private void ResizePriceInput(string newText)
		{
			var nsString = new NSString(newText);
			var attribs = new UIStringAttributes { Font = ItemPriceTextField.Font };
			var size = nsString.GetSizeUsingAttributes(attribs);

			if (!itemPriceTextFieldXPosInitialized)
			{
				itemPriceTextFieldXPosInitialized = true;
				itemPriceTextFieldXPos = ItemPriceTextField.Frame.X + ItemPriceTextField.Frame.Width;
			}

			ItemPriceTextField.Frame = new CGRect(itemPriceTextFieldXPos - size.Width, ItemPriceTextField.Frame.Y, size.Width, ItemPriceTextField.Frame.Height);
			ItemPriceTextField.TranslatesAutoresizingMaskIntoConstraints = true;
			ItemPriceTextField.SetNeedsDisplay();
		}

		private bool ItemPriceTextFieldShouldChangeCharacters(UITextField textField, NSRange range, string replacement)
		{
			var oldText = textField.Text;
			var newText = oldText.Substring(0, (int)range.Location) + replacement + oldText.Substring(((int)range.Location) + ((int)range.Length));

			if (newText.Length > 8)
			{
				return false;
			}

			ResizePriceInput(newText);

			return true;
		}

		private void ItemPriceTextFieldEditingDidEnd(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(ItemPriceTextField.Text))
			{
				ItemPriceTextField.Text = DefaultPriceText;
				ResizePriceInput();
			}
		}

		private void ApplyFocusedInputStyles(UIView input, bool focused)
		{
			UILabel label = null;
			UIView line = null;
			UIColor lineColor = focused ? UIColor.FromRGB(0xE1, 0x11, 0x8C) : UIColor.FromRGB(0xD0, 0xDB, 0xEC);
			UIColor labelColor = focused ? UIColor.FromRGB(0xE1, 0x11, 0x8C) : UIColor.FromRGB(0x00, 0x1D, 0x4D);
			string inputText = GetTextValueFromInput(input);

			if (input == ItemNameTextField)
			{
				label = ItemNameLabel;
				line = ItemNameLineView;
			}
			else if (input == ItemDescriptionTextView)
			{
				label = ItemDescriptionLabel;
				line = ItemDescriptionLineView;
			}

			if (label != null)
			{
				nfloat fontSize = focused || !string.IsNullOrEmpty(inputText) ? 10.0f : 12.0f;
				label.Font = UIFont.FromName(label?.Font.Name, fontSize);
				label.TextColor = labelColor;
			}

			if (line != null)
			{
				line.BackgroundColor = lineColor;
			}
		}

		private static string GetTextValueFromInput(UIView input)
		{
			string inputText = string.Empty;

			if (input is UITextView)
			{
				inputText = (input as UITextView).Text;
			}
			else if (input is UITextField)
			{
				inputText = (input as UITextField).Text;
			}

			return inputText;
		}

		private void OnTapGesture()
		{
			ItemNameTextField.ResignFirstResponder();
			ItemPriceTextField.ResignFirstResponder();
			ItemDescriptionTextView.ResignFirstResponder();
		}

		private void KeyboardWillShow(NSNotification notification)
		{
			ApplyFocusedInputStyles(lastResponder, false);

			lastResponder = View.GetFirstResponder();
			ApplyFocusedInputStyles(lastResponder, true);
			ScrollViewToShowCurrentResponder(notification);
		}

		private void KeyboardWillHide(NSNotification notification)
		{
			UIView responder = View.GetFirstResponder();
			ApplyFocusedInputStyles(responder, false);

			UIEdgeInsets contentInsets = UIEdgeInsets.Zero;
			ContentScrollView.ContentInset = contentInsets;
			ContentScrollView.ScrollIndicatorInsets = contentInsets;
		}

		private void ScrollViewToShowCurrentResponder(NSNotification notification)
		{
			var keyboardInfoValue = notification.UserInfo[UIKeyboard.FrameEndUserInfoKey] as NSValue;
			CGRect rawFrame = keyboardInfoValue.CGRectValue;
			CGRect keyboardFrame = View.ConvertRectFromView(rawFrame, null);
			nfloat keyboardHeight = keyboardFrame.Height;

			UIEdgeInsets contentInsets = new UIEdgeInsets(0, 0, keyboardHeight, 0);
			ContentScrollView.ContentInset = contentInsets;
			ContentScrollView.ScrollIndicatorInsets = contentInsets;

			CGRect aRect = View.Frame;
			aRect.Size = new CGSize(aRect.Size.Width, keyboardHeight);

			var responder = View.GetFirstResponder();

			if (!aRect.Contains(responder.Frame))
			{
				PointF scrollPoint = new PointF(0.0f, (float)responder.Frame.Y - (float)keyboardHeight);
				ContentScrollView.SetContentOffset(scrollPoint, true);
			}
		}

		private class TextInputBottomLineLayer : CALayer
		{
			private const float borderWith = 1.0f;

			public TextInputBottomLineLayer()
			{
				BorderWidth = borderWith;
			}
		}
	}
}