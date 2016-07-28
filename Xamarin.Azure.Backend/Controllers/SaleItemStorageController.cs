using Microsoft.Azure.Mobile.Server.Files;
using Microsoft.Azure.Mobile.Server.Files.Controllers;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Xamarin.Azure.Backend.DataObjects;

namespace Xamarin.Azure.Backend.Controllers
{
    public class SaleItemStorageController : StorageController<SaleItem>
    {
        // POST tables/SaleItem/F366EBB2-6D7D-4E09-A4F2-DF16819F6302/StorageToken
        [Route("tables/SaleItem/{id}/StorageToken")]
        public async Task<HttpResponseMessage> PostStorageTokenRequest(string id, StorageTokenRequest value)
        {
            StorageToken token = await GetStorageTokenAsync(id, value);

            return Request.CreateResponse(token);
        }

        // Get the files associated with this record
        // GET tables/SaleItem/F366EBB2-6D7D-4E09-A4F2-DF16819F6302/MobileServiceFiles
        [Route("tables/SaleItem/{id}/MobileServiceFiles")]
        public async Task<HttpResponseMessage> GetFiles(string id)
        {
            IEnumerable<MobileServiceFile> files = await GetRecordFilesAsync(id);

            return Request.CreateResponse(files);
        }

        // DELETE tables/SaleItem/F366EBB2-6D7D-4E09-A4F2-DF16819F6302/MobileServiceFiles/{name}
        [Route("tables/SaleItem/{id}/MobileServiceFiles/{name}")]
        public Task Delete(string id, string name)
        {
            return base.DeleteFileAsync(id, name);
        }
    }
}