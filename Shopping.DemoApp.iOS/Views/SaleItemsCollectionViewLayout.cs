using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Shopping.DemoApp.iOS
{
    public partial class SaleItemsCollectionViewLayout : UICollectionViewLayout
    {
        private const int NumberOfColumns = 2;

        private List<UICollectionViewLayoutAttributes> calculatedAttributes = new List<UICollectionViewLayoutAttributes>();
        private nfloat contentWidth = 0f;
        private nfloat contentHeight = 0f;
        private nfloat buttonHeight = 60f;

        private nfloat[] columnsXOffset = new nfloat[NumberOfColumns];
        private nfloat[] columnsYOffset = new nfloat[NumberOfColumns];

        public SaleItemsCollectionViewLayout (IntPtr handle) : base (handle)
        {
        }

        public override void PrepareLayout()
        {
            base.PrepareLayout();
            calculatedAttributes.Clear();

            contentWidth = CollectionView.Frame.Width;
            nfloat columnWidth = contentWidth / NumberOfColumns;
            nfloat height = columnWidth * 1.25f; // Our cells height is a 25% greater than their width

            for (int i = 0; i < columnsXOffset.Length; i++)
            {
                columnsXOffset[i] = i * columnWidth;
            }

            for (int i = 0; i < columnsYOffset.Length; i++)
            {
                columnsYOffset[i] = 0;
            }

            UICollectionViewLayoutAttributes attribute1 = UICollectionViewLayoutAttributes.CreateForCell(NSIndexPath.FromItemSection(0, 0));
            attribute1.Frame = new CGRect(columnsXOffset[1], 0, columnWidth, buttonHeight);
            calculatedAttributes.Add(attribute1);

            columnsYOffset[1] = buttonHeight; // Second column has a vertical offset (due to sell button height)

            int column = 0;
            for (int i = 0; i < CollectionView.NumberOfItemsInSection(1); i++)
            {
                var indexPath = NSIndexPath.FromItemSection(i, 1);
                var frame = new CGRect(columnsXOffset[column], columnsYOffset[column], columnWidth, height);
                var insetFrame = RectangleFExtensions.Inset(frame, 0f, 0f);

                UICollectionViewLayoutAttributes attribute = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
                attribute.Frame = insetFrame;

                calculatedAttributes.Add(attribute);

                contentHeight = System.NMath.Max(contentHeight, RectangleFExtensions.GetMaxY(frame));
                columnsYOffset[column] = columnsYOffset[column] + height;

                column = column >= (NumberOfColumns - 1) ? 0 : ++column;
            }
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            var attributes = new List<UICollectionViewLayoutAttributes>();

            foreach (var attr in calculatedAttributes)
            {
                CGRect intersection = CGRect.Intersect(attr.Frame, rect);

                if (intersection != CGRect.Empty)
                {
                    attributes.Add(attr);
                }
            }

            return attributes.ToArray();
        }

        public override CGSize CollectionViewContentSize
        {
            get
            {
                return new CGSize(contentWidth, contentHeight);
            }
        }
    }
}