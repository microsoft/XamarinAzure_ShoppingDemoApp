namespace Shopping.DemoApp.Services
{
    using Acr.UserDialogs;
    using Microsoft.WindowsAzure.MobileServices;
    using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
    using Microsoft.WindowsAzure.MobileServices.Sync;
    using Shopping.DemoApp.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.MobileServices.Files;
    using System.IO;
    using System.Linq;

    public class SaleItemDataService
    {
        private IMobileServiceSyncTable<SaleItem> saleItemsTable;
        private static SaleItemDataService instance = new SaleItemDataService();

        public static SaleItemDataService Instance
        {
            get
            {
                return instance;
            }
        }

        public MobileServiceClient MobileService { get; private set; }

        private SaleItemDataService()
        {
        }

        public async Task Initialize()
        {
            const string path = "syncstore.db";

            //Create our client
            this.MobileService = new MobileServiceClient(AppSettings.ApiAddress);
            //We add MobileServiceFileJsonConverter to the list of available converters to avoid an internal that occurs randomly
            this.MobileService.SerializerSettings.Converters.Add(new MobileServiceFileJsonConverter(this.MobileService));

            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<SaleItem>();

            //Get our sync table that will call out to azure
            this.saleItemsTable = this.MobileService.GetSyncTable<SaleItem>();

            // Add images handler
            this.MobileService.InitializeFileSyncContext(new ImagesFileSyncHandler(this.saleItemsTable), store);

            await this.MobileService.SyncContext.InitializeAsync(store, StoreTrackingOptions.NotifyLocalAndServerOperations);

        }

        public async Task<IEnumerable<SaleItem>> GetSaleItems()
        {
            await this.SyncSaleItems();

            return await this.saleItemsTable.OrderByDescending(c => c.CreatedAt).ToEnumerableAsync();
        }

        public async Task AddItemAsync(SaleItem item, string imagePath)
        {
            await this.saleItemsTable.InsertAsync(item);

            string targetPath = await FileHelper.CopySaleItemFileAsync(item.Id, imagePath);
            await this.saleItemsTable.AddFileAsync(item, Path.GetFileName(targetPath));

            await SyncSaleItems();
        }

        public async Task<MobileServiceFile> GetItemPhotoAsync(SaleItem item)
        {
            IEnumerable<MobileServiceFile> files = await this.saleItemsTable.GetFilesAsync(item);

            return files.FirstOrDefault();
        }

        public async Task SyncSaleItems()
        {   
            try
            {
                await this.MobileService.SyncContext.PushAsync();
                await this.saleItemsTable.PushFileChangesAsync();

                await this.saleItemsTable.PullAsync("allSaleItems", this.saleItemsTable.CreateQuery());
            }
#if __ANDROID__
            catch (Exception e)
            {
                Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
            }

#elif __IOS__
            catch (MobileServiceInvalidOperationException e)
            {
                Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
            }
#elif WINDOWS_UWP
            catch (Exception e)
            {
                Debug.WriteLine($"Sync Failed: {e.Message}");
            }
#else
            catch (Exception e)
            {
                Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
            }
#endif
        }

        public async Task BuySaleItemAsync(SaleItem item)
        {
            try
            {
                bool buySucceeded = await this.MobileService.InvokeApiAsync<SaleItem, bool>("buy", item);

                if (buySucceeded)
                {
                    await UserDialogs.Instance.AlertAsync("Thanks for buying this item");
                }
            }
#if __IOS__
            catch (MobileServiceInvalidOperationException e)
            {
                Console.Error.WriteLine(@"API invoking Failed: {0}", e.Message);
            }
#endif
            catch (Exception e)
            {
                Debug.WriteLine(@"Unexpected error {0}", e.Message);
            }
        }
    }
}
