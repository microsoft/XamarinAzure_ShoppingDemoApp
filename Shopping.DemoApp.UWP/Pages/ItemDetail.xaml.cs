using Shopping.DemoApp.Models;
using Shopping.DemoApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Shopping.DemoApp.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemDetail : Page
    {
        public SaleItem Item { get; set; }

        public ItemDetail()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = e.Parameter as SaleItem;

            this.imageItem.MaxHeight = Window.Current.Bounds.Height / 2;
        }

        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {   
            var item = this.DataContext as SaleItem;

            this.BuyButton.IsEnabled = false;
           
            await SaleItemDataService.Instance.BuySaleItemAsync(item);

            this.GoBack();
        }

        private void GoBack()
        {
            ((App)App.Current).RootFrame.GoBack();
        }
    }
}
