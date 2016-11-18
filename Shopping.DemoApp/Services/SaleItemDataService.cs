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

        public async Task SyncSaleItems()
        {
            try
            {
                await MobileService.SyncContext.PushAsync();
                await saleItemsTable.PushFileChangesAsync();

                await saleItemsTable.PullAsync("allSaleItems", this.saleItemsTable.CreateQuery());
            }

            catch (Exception e)
            {
                Debug.WriteLine(@"Sync Failed: {0}", e.Message);
            }
        }


        public async Task AddItemAsync(SaleItem item, string imagePath)
        {
            await saleItemsTable.InsertAsync(item);

            string targetPath = await FileHelper.CopySaleItemFileAsync(item.Id, imagePath);
            await saleItemsTable.AddFileAsync(item, Path.GetFileName(targetPath));

            await SyncSaleItems();
        }

        public async Task<MobileServiceFile> GetItemPhotoAsync(SaleItem item)
        {
            IEnumerable<MobileServiceFile> files = await this.saleItemsTable.GetFilesAsync(item);

            return files.FirstOrDefault();
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

            catch (Exception e)
            {
                Debug.WriteLine(@"Unexpected error {0}", e.Message);
            }
        }
    }
}
