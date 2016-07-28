namespace Shopping.DemoApp
{
	using System.IO;
	using System.Threading.Tasks;
	using PCLStorage;

	public static class FileHelper
	{
		private const string imagesFolder = "images";
		private const string imageFileName = "photo.jpg";

		public static async Task<string> CopySaleItemFileAsync(string itemId, string filePath)
		{   
			IFolder localStorage = FileSystem.Current.LocalStorage;

#if WINDOWS_UWP
            var targetPath = await GetLocalFilePathForUWPAsync(itemId);
#else
            var targetPath = await GetLocalFilePathAsync(itemId);
#endif
            var sourceFile = await FileSystem.Current.GetFileFromPathAsync(filePath);
            using (var sourceStream = await sourceFile.OpenAsync(PCLStorage.FileAccess.Read))
            {
                var targetFile = await localStorage.CreateFileAsync(targetPath, CreationCollisionOption.ReplaceExisting);
                using (var targetStream = await targetFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    await sourceStream.CopyToAsync(targetStream);
                }
            } 

			return targetPath;
		}

		public static async Task<string> GetLocalFilePathAsync(string itemId)
		{
			IFolder localStorage = FileSystem.Current.LocalStorage;

			var itemFolderPath = Path.Combine(imagesFolder, itemId);

			await localStorage.CreateFolderAsync(itemFolderPath, CreationCollisionOption.OpenIfExists);

			return Path.Combine(localStorage.Path, itemFolderPath, imageFileName);
		}

        public static async Task<string> GetLocalFilePathForUWPAsync(string itemId)
        {
            IFolder localStorage = FileSystem.Current.LocalStorage;

            var itemFolderPath = Path.Combine(imagesFolder, itemId);

            await localStorage.CreateFolderAsync(itemFolderPath, CreationCollisionOption.OpenIfExists);

            return Path.Combine(itemFolderPath, imageFileName);
        }
    }
}

