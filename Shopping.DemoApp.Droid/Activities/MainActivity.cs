using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Shopping.DemoApp.Droid.Fragments;
using Shopping.DemoApp.Droid.Adapter;
using Android.Support.V7.Widget;
using Acr.UserDialogs;
using Shopping.DemoApp.Droid.Services;
using Com.Lilarcor.Cheeseknife;
using System.Collections.Generic;
using Shopping.DemoApp.Models;
using Shopping.DemoApp.Services;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Support.V4.Content;
using Newtonsoft.Json;
using Shopping.DemoApp.Events;
using System.Linq;

namespace Shopping.DemoApp.Droid.Activities
{
	[Activity (Label = "Shopping Demo App",
        MainLauncher = true, 
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.saleitemgrid;

        [InjectView(Resource.Id.recyclerGridView)]
        protected RecyclerView recyclerView;

        private List<SaleItem> saleItems = new List<SaleItem>();

        private Random rnd = new Random();
        private SaleRecyclerAdapter saleRecyclerAdapter;

        private bool initialized = false;
        private bool askRatingShown = false;

        private bool refreshData = true;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Initialize();

            SetupUiElements();
        }

        protected async override void OnResume()
        {
            base.OnResume();

            if (!initialized)
            {
                await InitializeAsync();
                initialized = true;
            }

            if(refreshData)
            {
                await PopulateWithData();
                refreshData = false;
            }

            if (!askRatingShown)
            {
                await AskRating();
                askRatingShown = true;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            refreshData = true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //change main_compat_menu
            MenuInflater.Inflate(Resource.Menu.menu_home, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            
            switch (item.ItemId)
            {
                case Resource.Id.action_refresh:
                    PopulateWithData().ContinueWith((e) => { refreshData = false; });
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void Initialize()
        {
            UserDialogs.Init(this);

            AndroidPushService.Initialize(this);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
        }

        private async Task InitializeAsync()
        {
            await AndroidPushService.Register(null);
        }

        private async Task PopulateWithData()
        {
            UserDialogs.Instance.ShowLoading(maskType: MaskType.Clear);

            await SaleItemDataService.Instance.Initialize();
            var items = await SaleItemDataService.Instance.GetSaleItems();

            this.saleItems.Clear();
            this.saleItems.AddRange(items);
            var dummyItemPosition = this.saleItems.Count == 0 ? 0 : 1;
            this.saleItems.Insert(dummyItemPosition, default(SaleItem));

            this.saleRecyclerAdapter.NotifyDataSetChanged();

            SaleItemDataService.Instance.MobileService.EventManager
                .Subscribe<FileDownloadedEvent>(file =>
                {
                    this.RunOnUiThread(() =>
                    {
                        var itemPosition = this.saleItems.FindIndex(i => i != null && i.Id == file.Name);
                        if (itemPosition != -1)
                        {
                            this.saleRecyclerAdapter.NotifyItemChanged(itemPosition);
                        }
                    });
                });

            UserDialogs.Instance.HideLoading();
        }

        private async Task AskRating()
        {
            await Task.Delay(TimeSpan.FromSeconds(rnd.Next(3, 6)));
            if (this.IsActivityVisible)
            {
                var confirmed = await UserDialogs.Instance.ConfirmAsync("Time to rate the app! We'll calculate the score based on your smile.", okText: "Let's do it!");
                if (confirmed)
                {
                    this.StartActivity(typeof(SmileActivity));
                }
            }
        }

        private void SetupUiElements()
        {
            this.saleRecyclerAdapter = new SaleRecyclerAdapter(
                 this,
                 this.saleItems,
                 (v, i) =>
                 {
                     var currentSaleItem = this.saleItems[i];
                     var itemJson = JsonConvert.SerializeObject(currentSaleItem);

                     var detailIntent = new Intent(this, typeof(DetailSaleItemActivity));
                     detailIntent.PutExtra("Item", itemJson);

                     StartActivity(detailIntent);
                 },
                 async (v, i) =>
                 {
                     await AuthenticationService.Instance.RequestLoginIfNecessary();

                     if (AuthenticationService.Instance.UserIsAuthenticated)
                     {
                         StartActivityForResult(typeof(SellActivity), 0);
                     }
                 }
             );

            var sglm = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
            this.recyclerView.SetLayoutManager(sglm);
            this.recyclerView.SetItemAnimator(null);
            this.recyclerView.SetAdapter(this.saleRecyclerAdapter);

            var color = new Color(ContextCompat.GetColor(this, Resource.Color.grey));
            this.recyclerView.SetBackgroundColor(color);
        }
    }
}


