using Android.OS;
using Android.Views;
using Android.Widget;
using Shopping.DemoApp.Services;
using System.Linq;
using System.Threading.Tasks;
using XamarinUtils.Droid;
using System.IO;
using Shopping.DemoApp.Helpers;
using Shopping.DemoApp.Droid.Views;
using Android.Animation;
using Com.Lilarcor.Cheeseknife;
using Android.Views.Animations;
using Android.App;
using Acr.UserDialogs;
using Microsoft.ProjectOxford.Emotion.Contract;

namespace Shopping.DemoApp.Droid.Fragments
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SmileActivity : Activities.BaseActivity, View.IOnClickListener, ValueAnimator.IAnimatorUpdateListener
    {
        private const int StarAnimationInMillis = 2000;

        [InjectView(Resource.Id.takephotoview)]
        protected ImageView takePhotoImage;

        [InjectView(Resource.Id.smilepattern)]
        protected ImageView smilePattern;

        [InjectView(Resource.Id.repeatphotoview)]
        protected LinearLayout repeatPhotoView;

        [InjectView(Resource.Id.submitratingview)]
        protected LinearLayout submitRatingView;

        [InjectView(Resource.Id.postbuttons)]
        protected LinearLayout postButtons;

        [InjectView(Resource.Id.infotitletext)]
        protected TextView infoTitleText;

        [InjectView(Resource.Id.infodescription)]
        protected TextView infoDescriptionText;

        [InjectView(Resource.Id.overlay)]
        protected RelativeLayout overlay;

        [InjectView(Resource.Id.stars)]
        protected RelativeLayout stars;

        [InjectView(Resource.Id.cameraplaceholder)]
        protected RelativeLayout cameraLayout;

        private bool captureInProgress;
        private StarView starsView;
        private Camera2View cameraView;

        protected override int LayoutResource => Resource.Layout.smileview;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetupUiElements();
        }

        private void SetupUiElements()
        {
            this.takePhotoImage.SetOnClickListener(this);
            this.repeatPhotoView.SetOnClickListener(this);
            this.submitRatingView.SetOnClickListener(this);

            this.starsView = new StarView(this);
            this.stars.AddView(starsView);

            this.cameraView = new Camera2View(this);
            cameraLayout.AddView(cameraView);

            SetInitialState();

            cameraView.OpenFrontCamera();
        }

        private void SetInitialState()
        {
            this.infoTitleText.Text = RatingMsgHelper.GetInitialRatingTitle();
            this.starsView.Percentage = 0.0f;

            this.stars.Visibility = ViewStates.Invisible;
            this.takePhotoImage.Visibility = ViewStates.Visible;
            this.postButtons.Visibility = ViewStates.Invisible;
            this.smilePattern.Visibility = ViewStates.Visible;
            this.infoDescriptionText.Visibility = ViewStates.Invisible;
            this.overlay.Visibility = ViewStates.Invisible;
        }

        public async void OnClick(View view)
        {
            if(view == this.takePhotoImage)
            {
                await TakePhotoClick();
            }
            else if(view == this.repeatPhotoView)
            {
                RepeatPhotoClick();
            }
            else if (view == this.submitRatingView)
            {
                SubmitRatingClick();
            }
        }

        private void RepeatPhotoClick()
        {
            SetInitialState();

            cameraView.StartPreview();
        }

        private void SubmitRatingClick()
        {
            this.Finish();
        }

        private async Task TakePhotoClick()
        {
            if (!this.captureInProgress)
            {
                this.captureInProgress = true;

                var photoBytes = await this.cameraView.TakePicture();

                this.infoTitleText.Text = RatingMsgHelper.GetRecognizingRatingTitle();
                this.infoDescriptionText.Text = RatingMsgHelper.GetRecognizingRatingDescription();

                this.stars.Visibility = ViewStates.Visible;
                this.overlay.Visibility = ViewStates.Visible;
                this.takePhotoImage.Visibility = ViewStates.Invisible;
                this.smilePattern.Visibility = ViewStates.Invisible;
                this.infoDescriptionText.Visibility = ViewStates.Visible;

                using (var memoryStream = new MemoryStream(photoBytes))
                {
                    var emotionService = new EmotionService();
                    var firstFaceEmotion = default(Emotion);

                    try
                    {
                        var emotionList = await emotionService.RecognizeAsync(memoryStream);
                        firstFaceEmotion = emotionList.FirstOrDefault();
                    }
                    catch
                    {
                        await UserDialogs.Instance.AlertAsync("There was an error analyzing your photo. Please, try again.");
                    }

                    if (firstFaceEmotion != null)
                    {
                        var happiness = firstFaceEmotion.Scores.Happiness;

                        var textDescriptions = emotionService.GetTextsFor(happiness);

                        this.infoTitleText.Text = textDescriptions.Top;
                        this.infoDescriptionText.Text = textDescriptions.Message;

                        RunStarAnimation(happiness);
                    }
                    else
                    {
                        this.infoTitleText.Text = RatingMsgHelper.GetNoFaceDetectedTitle();
                        this.infoDescriptionText.Text = RatingMsgHelper.GetNoFaceDetectedMsg();
                    }

                    this.postButtons.Visibility = ViewStates.Visible;
                    this.overlay.Visibility = ViewStates.Invisible;
                }

                this.captureInProgress = false;
            }
        }

        private void RunStarAnimation(float happiness)
        {
            var animation = ValueAnimator.OfFloat(0.0f, happiness);
            animation.SetDuration(StarAnimationInMillis);
            animation.AddUpdateListener(this);
            animation.SetInterpolator(new OvershootInterpolator());
            animation.Start();
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            var starCurrentPercentage = (float)animation.AnimatedValue;
            this.starsView.Percentage = starCurrentPercentage;
        }
    }
}