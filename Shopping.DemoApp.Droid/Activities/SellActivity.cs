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
using System;
using Plugin.Media;
using Square.Picasso;
using Android.Graphics;
using Acr.UserDialogs;
using Shopping.DemoApp.Models;

namespace Shopping.DemoApp.Droid.Fragments
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SellActivity : Activities.BaseActivity, View.IOnClickListener
    {
        protected override int LayoutResource => Resource.Layout.sellview;

        [InjectView(Resource.Id.buttonTakePhotoLayout)]
        protected RelativeLayout buttonTakePhotoLayout;

        [InjectView(Resource.Id.discardImgLayout)]
        protected RelativeLayout discardImgLayout;

        [InjectView(Resource.Id.addItemText)]
        protected TextView addItemText;

        [InjectView(Resource.Id.photoImage)]
        protected ImageView photoImage;

        [InjectView(Resource.Id.priceEditText)]
        protected EditText priceEditText;

        [InjectView(Resource.Id.editInputTitle)]
        protected EditText editInputTitle;

        [InjectView(Resource.Id.editInputDescription)]
        protected EditText editInputDescription;

        private string currentImgPath;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetupUiElements();
        }

        private void SetupUiElements()
        {
            buttonTakePhotoLayout.SetOnClickListener(this);
            priceEditText.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if(string.IsNullOrWhiteSpace(priceEditText.Text))
                {
                    priceEditText.Text = "0.00";
                }
            };
            SetInitialState();
        }

        private void SetInitialState()
        {
            addItemText.Visibility = ViewStates.Visible;
            discardImgLayout.Visibility = ViewStates.Invisible;
            buttonTakePhotoLayout.Visibility = ViewStates.Visible;

            this.currentImgPath = null;

            photoImage.SetImageResource(Resource.Color.transparent);
        }

        public async void OnClick(View v)
        {
            if (v == buttonTakePhotoLayout)
            {
                await UploadPhoto();
            }
        }

        private async Task UploadPhoto()
        {
            await CrossMedia.Current.Initialize();
            var file = await CrossMedia.Current.PickPhotoAsync();
            if (file != null && file.Path != null)
            {
                currentImgPath = file.Path;

                Picasso.With(this)
                    .Load(new Java.IO.File(currentImgPath))
                    .Fit()
                    .CenterCrop()
                    .Error(Resource.Color.red)
                    .Placeholder(Resource.Color.grey)
                    .Into(photoImage);

                addItemText.Visibility = ViewStates.Invisible;
                discardImgLayout.Visibility = ViewStates.Visible;
                buttonTakePhotoLayout.Visibility = ViewStates.Invisible;
            }
        }

        [InjectOnClick(Resource.Id.buttonAddItem)]
        private async void OnAddItemClick(object sender, EventArgs e)
        {
            decimal price;

            if (!decimal.TryParse(priceEditText.Text, out price))
            {
                await UserDialogs.Instance.AlertAsync("Invalid price");
                return;
            }

            UserDialogs.Instance.ShowLoading(maskType: MaskType.Clear);

            try
            {
                var saleItem = new SaleItem
                {
                    Name = editInputTitle.Text,
                    Description = editInputDescription.Text,
                    Price = price,
                };

                await SaleItemDataService.Instance.AddItemAsync(saleItem, currentImgPath);

                Finish();
            }
            catch
            {
                await UserDialogs.Instance.AlertAsync("An error ocurred");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        [InjectOnClick(Resource.Id.discardImg)]
        private void OnDiscardPhotoItemClick(object sender, EventArgs e)
        {
            SetInitialState();
        }
    }
}