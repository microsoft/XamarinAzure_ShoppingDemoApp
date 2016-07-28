using System;
using Android.OS;
using Android.Views;
using Com.Lilarcor.Cheeseknife;
using Android.Widget;
using Shopping.DemoApp.Models;
using System.Globalization;
using Shopping.DemoApp.Services;
using Shopping.DemoApp.Droid.Activities;
using Android.App;
using Newtonsoft.Json;
using Square.Picasso;
using System.Threading.Tasks;
using Java.IO;

namespace Shopping.DemoApp.Droid.Fragments
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DetailSaleItemActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.detailsaleitem;

        [InjectView(Resource.Id.textTitle)]
        protected TextView textTitle;

        [InjectView(Resource.Id.textPriceCurrency)]
        protected TextView textPriceCurrency;

        [InjectView(Resource.Id.textPrice)]
        protected TextView textPrice;

        [InjectView(Resource.Id.textViewDecimals)]
        protected TextView textPriceDecimals;

        [InjectView(Resource.Id.itemDescription)]
        protected TextView itemDescription;

        [InjectView(Resource.Id.img)]
        protected ImageView img;

        private SaleItem saleItem;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HandleIntentParameters();

            SetupUiElements();
        }

        protected override async void OnResume()
        {
            base.OnResume();

            await BindImage(this.saleItem, img);
        }

        private void HandleIntentParameters()
        {
            var itemJson = Intent.GetStringExtra("Item");
            this.saleItem = JsonConvert.DeserializeObject<SaleItem>(itemJson);
        }

        private void SetupUiElements()
        {
            var priceValue = Math.Truncate(this.saleItem.Price);
            var decimals = this.saleItem.Price - priceValue;

            textTitle.Text = this.saleItem.Name.ToUpperInvariant();
            itemDescription.Text = this.saleItem.Description;

            textPriceCurrency.Text = "$"; //NumberFormat.CurrencySymbol
            textPrice.Text = priceValue.ToString(CultureInfo.InvariantCulture);
            textPriceDecimals.Text = decimals.ToString("0.00", CultureInfo.InvariantCulture).Substring(1);
        }

        private async Task BindImage(SaleItem item, ImageView imageView)
        {
            var picasso = Picasso.With(this);

            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                var requestCreator = picasso.Load(item.ImageUrl);
                LoadImage(requestCreator, imageView);
            }
            else
            {
                var localFile = await FileHelper.GetLocalFilePathAsync(item.Id);
                var requestCreator = picasso.Load(new File(localFile));
                LoadImage(requestCreator, imageView);
            }
        }

        private void LoadImage(RequestCreator requestCreator, ImageView imageView)
        {
            requestCreator.Fit()
                .CenterCrop()
                .Error(Resource.Color.transparent)
                .Placeholder(Resource.Color.grey)
                .Into(imageView);
        }

        [InjectOnClick(Resource.Id.buyButton)]
		private async void OnBuyButtonClick(object sender, EventArgs e)
        {
            await SaleItemDataService.Instance.BuySaleItemAsync(this.saleItem);
		}
    }
}