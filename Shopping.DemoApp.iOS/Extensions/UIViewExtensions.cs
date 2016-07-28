using System.Threading.Tasks;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using SDWebImage;
using Shopping.DemoApp.Events;
using Shopping.DemoApp.Models;
using Shopping.DemoApp.Services;
using UIKit;

namespace Shopping.DemoApp.iOS.Extensions
{
	public static class UIViewExtensions
	{
		public static UIView GetFirstResponder(this UIView parent)
		{
			UIView responder = null;

			if (parent.IsFirstResponder)
			{
				responder = parent;
			}
			else
			{
				foreach (UIView subView in parent.Subviews)
				{
					responder = GetFirstResponder(subView);

					if (responder != null)
					{
						break;
					}
				}
			}

			return responder;
		}

        public static async Task BindImageViewAsync(this UIImageView imageView, 
                                                    SaleItem saleItem)
        {
            // Manually uploaded items use Azure File sync
            if (string.IsNullOrEmpty(saleItem.ImageUrl))
            {
                await LoadImageAsync(saleItem, imageView);
            }
            // Default app images should have a public ImageUrl
            else
            {
                var url = new NSUrl(saleItem.ImageUrl);
                imageView.SetImage(url);
            }
        }

        private static async Task LoadImageAsync(SaleItem saleItem, 
                                                 UIImageView imageView)
        {
			string localFile = await FileHelper.GetLocalFilePathAsync(saleItem.Id);
			var image = UIImage.FromFile(localFile);

			NSUrl url = NSUrl.FromFilename(localFile);

			if (image != null)
			{
				imageView.SetImage(url, null, SDWebImageOptions.RefreshCached);
			}
			else
			{
				// We manually force the ImageView to be reset
				imageView.BeginInvokeOnMainThread(() => { imageView.Image = null; });
			}
        }
	}
}

