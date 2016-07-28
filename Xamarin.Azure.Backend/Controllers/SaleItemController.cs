using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Xamarin.Azure.Backend.DataObjects;
using Xamarin.Azure.Backend.Models;

namespace Xamarin.Azure.Backend.Controllers
{
    public class SaleItemController : TableController<SaleItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<SaleItem>(context, Request);
        }

        // GET tables/SaleItem
        public IQueryable<SaleItem> GetAllSaleItems()
        {
            return Query();
        }

        // GET tables/SaleItem/F366EBB2-6D7D-4E09-A4F2-DF16819F6302
        public SingleResult<SaleItem> GetSaleItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/SaleItem/F366EBB2-6D7D-4E09-A4F2-DF16819F6302
        public Task<SaleItem> PatchSaleItem(string id, Delta<SaleItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/SaleItem
        public async Task<IHttpActionResult> PostSaleItem(SaleItem item)
        {
            SaleItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/SaleItem/F366EBB2-6D7D-4E09-A4F2-DF16819F6302
        public Task DeleteSaleItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}