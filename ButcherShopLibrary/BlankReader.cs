using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;


namespace ButcherShopLibrary
{
    public class BlankReader
    {
        private string settingPath;

        #region Properties

        public string SettingFilePath
        {
            get { return settingPath; }
            set { settingPath = value; }
        }

        #endregion

        #region Methods

        public ObservableCollection<ProductCategory> ProductsOfBlank()
        {
            ObservableCollection<ProductCategory> categories = new ObservableCollection<ProductCategory>();

            if (settingPath == string.Empty)
                return categories;


            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(settingPath);

            XmlElement xRoot = xmlDocument.DocumentElement;
            if (xRoot != null)
            {
                foreach (XmlElement xNodeCategory in xRoot)
                {
                    ProductCategory category = new ProductCategory();

                    category.Id = Guid.Parse(xNodeCategory.Attributes.GetNamedItem("id").Value);
                    category.Name = xNodeCategory.Attributes.GetNamedItem("name").Value;

                    foreach (XmlNode xNodeProduct in xNodeCategory.ChildNodes)
                    {
                        Product product = new Product();

                        foreach (XmlNode productChildNode in xNodeProduct.ChildNodes)
                        {
                            if (productChildNode.Name == "id")
                                product.Id = Guid.Parse(productChildNode.InnerText);

                            if (productChildNode.Name == "name")
                                product.Name = productChildNode.InnerText;
                        }

                        category.Products.Add(product);
                    }
                    categories.Add(category);
                }
            }

            return categories;
        }

        public List<Product> ParseBlank(string blankPath)
        {
            if (blankPath == string.Empty)
                throw new ArgumentException("Путь к файлу бланка не может быть пустым.");


            List<Product> dataBlank = new List<Product>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(blankPath);

            XmlElement xRoot = xmlDocument.DocumentElement;
            if (xRoot != null)
            {
                XmlNode xNodeCategory = xRoot.ChildNodes[0];
                foreach (XmlNode xNodeProduct in xNodeCategory.ChildNodes)
                {
                    Product product = new Product();

                    foreach (XmlNode productChildNode in xNodeProduct.ChildNodes)
                    {
                        if (productChildNode.Name == "id")
                            product.Id = Guid.Parse(productChildNode.InnerText);

                        if (productChildNode.Name == "name")
                            product.Name = productChildNode.InnerText;

                        if (productChildNode.Name == "count")
                            product.Count = double.Parse(productChildNode.InnerText, CultureInfo.InvariantCulture);
                    }

                    dataBlank.Add(product);
                }
            }

            return dataBlank;
        }

        #endregion
    }
}