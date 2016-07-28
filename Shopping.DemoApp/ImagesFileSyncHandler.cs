namespace Shopping.DemoApp
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.MobileServices.Eventing;
    using Microsoft.WindowsAzure.MobileServices.Files;
    using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
    using Microsoft.WindowsAzure.MobileServices.Files.Sync;
    using Microsoft.WindowsAzure.MobileServices.Sync;
    using PCLStorage;
    using Shopping.DemoApp.Events;
    using Shopping.DemoApp.Models;
    using Shopping.DemoApp.Services;


    public class ImagesFileSyncHandler : IFileSyncHandler
    {
        private static object currentDownloadTaskLock = new object();
        private static Task currentDownloadTask = Task.FromResult(0);

        private IMobileServiceSyncTable<SaleItem> saleItemsTable;

        public ImagesFileSyncHandler(IMobileServiceSyncTable<SaleItem> table)
        {
            saleItemsTable = table;
        }

        public async Task<IMobileServiceFileDataSource> GetDataSource(MobileServiceFileMetadata metadata)
        {
            string filePath = await FileHelper.GetLocalFilePathAsync(metadata.ParentDataItemId);

            return new PathMobileServiceFileDataSource(filePath);
        }

        public async Task ProcessFileSynchronizationAction(MobileServiceFile file, FileSynchronizationAction action)
        {
            if (action == FileSynchronizationAction.Delete)
            {
                // You can remove images for delete items here
            }
            else
            {
                await DownloadFileAsync(file);
            }
        }

        private Task DownloadFileAsync(MobileServiceFile file)
        {
            lock (currentDownloadTaskLock)
            {
                return currentDownloadTask =
                    currentDownloadTask.ContinueWith(x => DoFileDownloadAsync(file)).Unwrap();
            }
        }

        private async Task DoFileDownloadAsync(MobileServiceFile file)
        {
            var filepath = await FileHelper.GetLocalFilePathAsync(file.ParentId);

            var storagePath = FileSystem.Current.LocalStorage.Path;

            var fullpath = Path.Combine(storagePath, filepath);

            using (System.IO.Stream stream = File.Create(fullpath)) { }

            await saleItemsTable.DownloadFileAsync(file, fullpath);

            await SaleItemDataService.Instance.MobileService.EventManager
                                     .PublishAsync(new FileDownloadedEvent { Name = file.ParentId });
        }
    }
}

