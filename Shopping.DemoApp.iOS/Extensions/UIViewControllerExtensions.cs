using System;
using UIKit;

namespace Shopping.DemoApp.iOS.Extensions
{
	public static class UIViewControllerExtensions
	{
		public static void AddDefaultNavigationTitle(this UIViewController controller)
		{
			var logoImage = UIImage.FromFile("logo.png");
			var logoView = new UIImageView(logoImage);
			logoView.ContentMode = UIViewContentMode.ScaleAspectFit;

			var titleView = new UIView();
			titleView.Frame = new CoreGraphics.CGRect(0, 0, 200, 44);
			logoView.Frame = titleView.Bounds;
			titleView.AddSubview(logoView);

			controller.NavigationItem.TitleView = titleView;
		}
	}
}