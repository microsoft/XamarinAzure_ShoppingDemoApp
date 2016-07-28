namespace Shopping.DemoApp.Models
{
    using System;

    public class SaleItem
    {
        public string Id { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}
