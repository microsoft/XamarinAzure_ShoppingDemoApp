using Acr.UserDialogs;
using Foundation;
using Microsoft.ProjectOxford.Emotion.Contract;
using Shopping.DemoApp.Models;
using Shopping.DemoApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace Shopping.DemoApp.iOS.Controllers
{
    public partial class RatingProcessingViewController : UIViewController
    {
        private RatingStarsController ratingController;
		private readonly EmotionService emotionClient;

        public RatingProcessingViewController(IntPtr handle) : base(handle)
        {
			emotionClient = new EmotionService();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			ratingController = ChildViewControllers.OfType<RatingStarsController>().FirstOrDefault();
            RepeatPhotoButton.TouchUpInside += RepeatPhoto;
            SubmitRatingButton.TouchUpInside += SubmitRating;
        }

		public async void AnalyzeCapture()
		{
			try
			{
				UserDialogs.Instance.ShowLoading("Processing...");

				UIImage sendImage = UIImage.FromImage(CaptureImageView.Image.CGImage, 1.0f, UIImageOrientation.Right);

				Emotion[] detectedEmotions = await emotionClient.RecognizeAsync(sendImage.AsJPEG().AsStream());
				Emotion emotion = detectedEmotions.FirstOrDefault();

				UserDialogs.Instance.HideLoading();

				if (emotion != null)
				{
					SetRating(emotion.Scores.Happiness);
				}
				else
				{
					await UserDialogs.Instance.AlertAsync("No face detected. Please, try again.");
				}
			}
			catch
			{
				UserDialogs.Instance.HideLoading();
				await UserDialogs.Instance.AlertAsync("There was an error analyzing your photo. Please, try again.");
			}
		}

        public void SetCaptureImage(NSData imageData)
        {
			TopLabel.Text = "Please, wait a few seconds for us to run facial recognition";
			MessageLabel.Text = string.Empty;

			if (imageData != null)
            {
				UIImage image = UIImage.LoadFromData(imageData);

				// we need to flip image to present it in correct orientation
				UIImage flippedImage = UIImage.FromImage(image.CGImage, 1.0f, UIImageOrientation.LeftMirrored);

                CaptureImageView.Image = flippedImage;
            }
        }

        private void RepeatPhoto(object sender, EventArgs e)
        {
            RatingMainViewController parentController = ParentViewController as RatingMainViewController;

            parentController?.ActivateCapture();
        }

        private async void SubmitRating(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Submitting rating...");

            // Here you can write your own code to save user ratings
            await Task.Delay(1000);

            UserDialogs.Instance.HideLoading();

            NavigationController.PopViewController(true);
        }

        private void SetRating(float rating)
        {
            ratingController.SetRating(rating);

            RatingText text = emotionClient.GetTextsFor(rating);
            TopLabel.Text = text.Top;
            MessageLabel.Text = text.Message;
        }
    }
}