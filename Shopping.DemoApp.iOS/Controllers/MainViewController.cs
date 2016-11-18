using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Foundation;
using Shopping.DemoApp.Events;
using Shopping.DemoApp.iOS.Extensions;
using Shopping.DemoApp.Models;
using Shopping.DemoApp.Services;
using UIKit;

namespace Shopping.DemoApp.iOS.Controllers
{
	public partial class MainViewController : UIViewController
    {
        private const int RatingAlertDelaySeconds = 2;
        private SaleItemsDataSource saleItemsSource;

        public MainViewController (IntPtr handle) : base (handle)
		{
		}

        public async override void ViewDidLoad ()
        {
            base.ViewDidLoad();

			this.AddDefaultNavigationTitle();
            CustomizeNavigationBar();

            await InitializeCollectionView();
            await LoadSaleItems();

            NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(RatingAlertDelaySeconds), OnRatingAlertScheduled);
        }

        public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }

        private void CustomizeNavigationBar()
        {
            NavigationController.NavigationBar.Translucent = false;
            NavigationController.NavigationBar.BarTintColor = new UIColor(1 / 255f, 31 / 255f, 79 / 255f, 1);
            NavigationController.NavigationBar.TintColor = UIColor.White;
            NavigationController.NavigationBar.ShadowImage = new UIImage();
			NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);

			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
			{
				ForegroundColor = UIColor.White
			};

            NavigationItem.RightBarButtonItem = 
                new UIBarButtonItem(UIBarButtonSystemItem.Refresh, async delegate
            {
                await LoadSaleItems();
            });
        }

        private async Task InitializeCollectionView()
        {
            UINib nib = UINib.FromName(SaleItemViewCell.Key, null);
            SaleItemsCollectionView.RegisterNibForCell(nib, SaleItemViewCell.Key);

            nib = UINib.FromName(SellArticleButtonViewCell.Key, null);
            SaleItemsCollectionView.RegisterNibForCell(nib, SellArticleButtonViewCell.Key);

            saleItemsSource = new SaleItemsDataSource();
            saleItemsSource.SellRequestedCallback = OnSellRequested;
			saleItemsSource.SaleItemSelectedCallback = OnSaleItemSelected;
            SaleItemsCollectionView.Source = saleItemsSource;

			await SaleItemDataService.Instance.Initialize();

			// As each item image download may occur after its cell is loaded,
			// we should reload it when that download is completed
			SaleItemDataService.Instance.MobileService.EventManager
								   .Subscribe<FileDownloadedEvent>(file =>
				{
					SaleItemsCollectionView.BeginInvokeOnMainThread(() =>
					{
						var targetCell = SaleItemsCollectionView.VisibleCells
																.OfType<SaleItemViewCell>()
																.FirstOrDefault(c => c.ItemId == file.Name);

						if (targetCell != null)
						{
							NSIndexPath path = SaleItemsCollectionView.IndexPathForCell(targetCell);
							SaleItemsCollectionView.ReloadItems(new[] { path });
						}
					});
				});
        }

		private async Task LoadSaleItems()
        {
            UserDialogs.Instance.ShowLoading("Loading...");

            IEnumerable<SaleItem> data = await SaleItemDataService.Instance.GetSaleItems();
            saleItemsSource.Items = data;
            SaleItemsCollectionView.ReloadData();

            UserDialogs.Instance.HideLoading();
        }

        private async void OnRatingAlertScheduled(NSTimer timer)
        {
            timer.Invalidate();
            timer.Dispose();
            timer = null;

            bool confirmed = await UserDialogs.Instance.ConfirmAsync("Time to rate the app! We'll calculate the score based on your smile.", okText: "Let's do it!");

            if (confirmed)
            {
                UIViewController controller = Storyboard.InstantiateViewController(nameof(RatingMainViewController));
                NavigationController.PushViewController(controller, true);
            }
        }

        private async void OnSellRequested()
        {
			await AuthenticationService.Instance.RequestLoginIfNecessary();

            if (AuthenticationService.Instance.UserIsAuthenticated)
            {
                UIViewController controller = Storyboard.InstantiateViewController(nameof(AddSaleItemViewController));
                NavigationController.PushViewController(controller, true);
            }
        }

		private void OnSaleItemSelected(SaleItem item)
		{
			ItemDetailViewController controller = Storyboard.InstantiateViewController(nameof(ItemDetailViewController)) as ItemDetailViewController;
			controller.SaleItem = item;

			NavigationController.PushViewController(controller, true);
		}
    }
}

