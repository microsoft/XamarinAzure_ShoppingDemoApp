using Foundation;
using Shopping.DemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Shopping.DemoApp.iOS
{
    public class SaleItemsDataSource : UICollectionViewSource
    {
        public IEnumerable<SaleItem> Items { get; set; }

        public Action SellRequestedCallback { get; set; }

		public Action<SaleItem> SaleItemSelectedCallback { get; set; }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 2;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            int count = section == 0 ? 1 : Items?.Count() ?? 0;

            return count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                SellArticleButtonViewCell cell = collectionView.DequeueReusableCell(SellArticleButtonViewCell.Key, indexPath) as SellArticleButtonViewCell ?? SellArticleButtonViewCell.Create();
                cell.AdjustIconAspect();

                return cell;
            }
            else
            {
                SaleItemViewCell cell = collectionView.DequeueReusableCell(SaleItemViewCell.Key, indexPath) as SaleItemViewCell ?? SaleItemViewCell.Create();

                cell.Bind(Items.ElementAt(indexPath.Row));

                return cell;
            }
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
			if (indexPath.Section == 0)
			{
				SellRequestedCallback?.Invoke();
			}
			else if (indexPath.Section == 1)
			{
				SaleItem item = Items.ElementAt(indexPath.Row);
				SaleItemSelectedCallback?.Invoke(item);
			}
        }
    }
}
