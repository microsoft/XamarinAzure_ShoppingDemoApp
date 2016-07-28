#pragma warning disable SA1652 // Enable XML documentation output
// <copyright file="MainPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Shopping.DemoApp.UWP
#pragma warning restore SA1652 // Enable XML documentation output
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Acr.UserDialogs;
    using Shopping.DemoApp.Helpers;
    using Shopping.DemoApp.Services;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;
    using System.Threading.Tasks;
    using Pages;
    using Windows.Networking.PushNotifications;
    using Events;
    using System.Diagnostics;
    using Microsoft.WindowsAzure.MobileServices;
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool reloadItems;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (this.saleItemList.ItemsSource == null)
            {
                await SaleItemDataService.Instance.Initialize();

                await this.InitNotificationsAsync();

                await this.UpdateList();

                SaleItemDataService.Instance.MobileService.EventManager.Subscribe<FileDownloadedEvent>(async file =>
                {
                    Debug.WriteLine($"downloaded file {file.Name}");

                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.saleItemList.UpdateItem(file.Name);
                    });
                });

                await Task.Delay(3000);

                Dialog.ShowRating(this.GoToRate);
            }
            else
            {
                if (this.reloadItems)
                {
                    this.reloadItems = false;
                    await this.UpdateList();
                }
            }
        }

        private void GoToRate(bool donavigation)
        {
            if (donavigation)
            {
                (App.Current as App).RootFrame.Navigate(typeof(Rating));
            }
        }

        private async Task UpdateList()
        {
            VisualStateManager.GoToState(this, LoadingState.Name, true);

            var items = await SaleItemDataService.Instance.GetSaleItems();

            this.saleItemList.ItemsSource = items;

            VisualStateManager.GoToState(this, LoadedState.Name, true);
        }

        private async void saleItemList_NavigateToSellArticle(object sender, EventArgs e)
        {
            bool loginSucceded = await AuthenticationService.Instance.RequestLoginIfNecessary();

            if (loginSucceded)
            {
                this.reloadItems = true;
                (App.Current as App).RootFrame.Navigate(typeof(AddItem));
            }

        }

        private async Task InitNotificationsAsync()
        {
            try
            {
                // Get a channel URI from WNS.
                var channel = await Windows.Networking.PushNotifications.PushNotificationChannelManager
                    .CreatePushNotificationChannelForApplicationAsync();

                await SaleItemDataService.Instance.MobileService.GetPush().RegisterAsync(channel.Uri);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception registering notifications channel. " + ex);
            }
        }

        private void navigateToItemDetail(object sender, UserControls.ItemSelectedEventArgs e)
        {
            (App.Current as App).RootFrame.Navigate(typeof(ItemDetail), e.Item);
        }
    }
}
