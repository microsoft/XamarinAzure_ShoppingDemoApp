using CoreGraphics;
using System;
using UIKit;

namespace Shopping.DemoApp.iOS.Controllers
{
    public partial class RatingStarsController : UIViewController
    {
        private UIView maskView;

        public RatingStarsController (IntPtr handle) : base (handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            maskView = new UIView(FullStarsContainerView.Frame)
            {
                BackgroundColor = UIColor.White
            };
            maskView.Layer.AnchorPoint = CGPoint.Empty;
            FullStarsContainerView.MaskView = maskView;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            maskView.Frame = FullStarsContainerView.Frame;
            ResetStarsToZero();
        }

        public void SetRating (float rating)
        {
            ResetStarsToZero();

            UIView.BeginAnimations("animateStars");
            UIView.SetAnimationDuration(2);
            UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);

            maskView.Bounds = new CGRect(0, 0, FullStarsContainerView.Frame.Width * rating, FullStarsContainerView.Frame.Height);

            UIView.CommitAnimations();
        }

        private void ResetStarsToZero()
        {
            var bounds = maskView.Bounds;
            bounds.Width = 0;
            maskView.Bounds = bounds;
        }
   }
}