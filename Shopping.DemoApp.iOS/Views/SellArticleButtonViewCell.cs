using Foundation;
using System;
using UIKit;

namespace Shopping.DemoApp.iOS
{
    public partial class SellArticleButtonViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("SellArticleButtonViewCell");
        public static readonly UINib Nib;

        static SellArticleButtonViewCell()
        {
            Nib = UINib.FromName("SellArticleButtonViewCell", NSBundle.MainBundle);
        }

        protected SellArticleButtonViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static SellArticleButtonViewCell Create()
        {
            SellArticleButtonViewCell cell = (SellArticleButtonViewCell)Nib.Instantiate(null, null)[0];
                        
            return cell;
        }

        public void AdjustIconAspect()
        {
            SellButton.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
        }
    }
}