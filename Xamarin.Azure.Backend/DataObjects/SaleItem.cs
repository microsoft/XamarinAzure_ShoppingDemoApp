using Microsoft.Azure.Mobile.Server;

namespace Xamarin.Azure.Backend.DataObjects
{
    public class SaleItem : EntityData
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}