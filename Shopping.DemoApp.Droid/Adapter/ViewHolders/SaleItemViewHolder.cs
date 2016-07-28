using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;
using Shopping.DemoApp.Droid.Adapter.ViewHolders;

namespace Shopping.DemoApp.Droid.Adapter
{
    public class SaleItemViewHolder : BaseViewHolder
    {
        [InjectView(Resource.Id.displayName)]
        public TextView DisplayName { get; set; }

        [InjectView(Resource.Id.description)]
        public TextView Description { get; set; }

        [InjectView(Resource.Id.price)]
        public TextView Price { get; set; }

        [InjectView(Resource.Id.priceCurrency)]
        public TextView PriceCurrency { get; set; }

        [InjectView(Resource.Id.imgThumbnail)]
        public ImageView Thumbnail { get; set; }

        public SaleItemViewHolder(View itemView) : base(itemView)
        {
        }
    }
}