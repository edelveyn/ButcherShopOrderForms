using System;
using System.Collections.ObjectModel;

namespace ButcherShopLibrary
{
    public class ProductCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Product> Products { get; set; }

        #region Constructors

        public ProductCategory()
        {
            Products = new ObservableCollection<Product>();
        }

        #endregion
    }
}