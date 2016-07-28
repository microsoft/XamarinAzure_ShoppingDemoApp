using Shopping.DemoApp.Helpers;
using Shopping.DemoApp.Models;
using Shopping.DemoApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using PCLStorage;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Shopping.DemoApp.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddItem : Page
    {
        private SaleItem item;
        private string itemImagePath;
        private StorageFile storageFile;

        public AddItem()
        {
            this.InitializeComponent();
            this.Loaded += AddItem_Loaded;
        }

        public void CheckPriceIsEmpty()
        {
            decimal price;

            if (!decimal.TryParse(this.price.Text, out price))
            {
                this.price.Text = "0.00";
            }
            else
            {
                this.price.Text = price.ToString("0.##");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.item = new SaleItem();
        }
        private void AddItem_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, NoPhoto.Name, true);
        }

        private async void SellClick(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, Selling.Name, true);          

            try
            {
                this.item.Name = this.name.Text;
                this.item.Description = this.GetTextDescription(this.description);
                this.item.Price = decimal.Parse(this.price.Text);

                this.itemImagePath = await this.CopyStorageFile(this.storageFile);

                await SaleItemDataService.Instance.AddItemAsync(this.item, this.itemImagePath);

                this.GoBack();
            }
            catch
            {
                await Dialog.AlertAsync("An error ocurred");
            }
            finally
            {
                Dialog.HideLoading();
            }        
        }

        private async Task<string> CopyStorageFile(StorageFile storageFile)
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            var copiedFile = await storageFile.CopyAsync(localFolder, "tmpPhoto.jpg", Windows.Storage.NameCollisionOption.ReplaceExisting);

            return copiedFile.Path;
        }

        private string GetTextDescription(RichEditBox rtb)
        {
            string text = string.Empty;
            rtb.Document.GetText(TextGetOptions.None, out text);

            return text;
        }

        private async void AddPhoto(object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".jpeg");
            fileOpenPicker.FileTypeFilter.Add(".bmp");

            this.storageFile = await fileOpenPicker.PickSingleFileAsync();

            if (storageFile != null)
            {
                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    BitmapImage bitmapImage = new BitmapImage();

                    await bitmapImage.SetSourceAsync(fileStream);
                    this.itemImage.Source = bitmapImage;
                    this.itemImagePath = storageFile.Path;
                    VisualStateManager.GoToState(this, Photo.Name, true);
                }
            }
        }

        private void RemovePhotoItem(object sender, RoutedEventArgs e)
        {
            this.itemImagePath = string.Empty;
            this.itemImage.Source = null;
            VisualStateManager.GoToState(this, NoPhoto.Name, true);
        }

        private void GoBack()
        {
            ((App)App.Current).RootFrame.GoBack();
        }
    }
}
