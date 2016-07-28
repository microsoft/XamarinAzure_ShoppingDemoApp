using System;
using Foundation;
using Shopping.DemoApp.iOS.Extensions;
using Shopping.DemoApp.Models;
using Shopping.DemoApp.Services;
using UIKit;

namespace Shopping.DemoApp.iOS.Controllers
{
    public partial class ItemDetailViewController : UIViewController
	{
		public SaleItem SaleItem { get; set; }

		public ItemDetailViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.AddDefaultNavigationTitle();

			LoadData();
            BuyButton.TouchUpInside += OnBuyRequested;
		}

        private async void LoadData()
		{
            await ItemImageView.BindImageViewAsync(SaleItem);

            ItemTitleLabel.Text = (SaleItem.Name ?? string.Empty).ToUpper();
			ItemDescriptionLabel.Text = SaleItem.Description;

			var smallAttributes = new UIStringAttributes
			{
				Font = UIFont.FromName(ItemPriceLabel.Font.Name, 20f)
			};

			string priceStr = "$" + SaleItem.Price.ToString("0.00");
			NSMutableAttributedString mutablePriceStr = new NSMutableAttributedString(priceStr);

			mutablePriceStr.SetAttributes(smallAttributes.Dictionary, new NSRange(0, 1));
			mutablePriceStr.SetAttributes(smallAttributes.Dictionary, new NSRange(priceStr.Length - 3, 3));

			ItemPriceLabel.AttributedText = mutablePriceStr;
        }

        private async void OnBuyRequested(object sender, EventArgs e)
        {
			await SaleItemDataService.Instance.BuySaleItemAsync(SaleItem);
        }
    }
}