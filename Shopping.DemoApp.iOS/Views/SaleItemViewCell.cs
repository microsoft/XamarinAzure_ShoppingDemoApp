using Foundation;
using SDWebImage;
using Shopping.DemoApp.Models;
using System;
using System.Globalization;
using UIKit;
using System.Threading.Tasks;
using Shopping.DemoApp.Services;
using Shopping.DemoApp.Events;
using Shopping.DemoApp.iOS.Extensions;

namespace Shopping.DemoApp.iOS
{
    public partial class SaleItemViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("SaleItemViewCell");
        public static readonly UINib Nib;

		public string ItemId { get; private set; }

        static SaleItemViewCell()
        {
            Nib = UINib.FromName("SaleItemViewCell", NSBundle.MainBundle);
        }

        protected SaleItemViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static SaleItemViewCell Create()
        {
            SaleItemViewCell cell = (SaleItemViewCell)Nib.Instantiate(null, null)[0];

            return cell;
        }

        public async void Bind(SaleItem saleItem)
        {
            await ItemImageView.BindImageViewAsync(saleItem);

            ItemNameLabel.Text = !string.IsNullOrEmpty(saleItem.Name) ? saleItem.Name.ToUpperInvariant() : string.Empty;
            ItemDescriptionLabel.Text = saleItem.Description;

            var smallAttributes = new UIStringAttributes
            {
                Font = UIFont.FromName(ItemPriceLabel.Font.Name, 10f)
            };

            string priceStr = "$" + Math.Round(saleItem.Price);
            NSMutableAttributedString mutablePriceStr = new NSMutableAttributedString(priceStr);

            mutablePriceStr.SetAttributes(smallAttributes.Dictionary, new NSRange(0, 1));

            ItemPriceLabel.AttributedText = mutablePriceStr;
			ItemId = saleItem.Id;
        }
    }
}