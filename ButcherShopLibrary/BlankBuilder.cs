using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ButcherShopLibrary
{
    public class BlankBuilder
    {
        public string ShopKey { get; set; }
        public ProductCategory Category{ get; set; }

        #region Constructors

        public BlankBuilder() { }

        public BlankBuilder(string shopKey, ProductCategory category)
        {
            ShopKey = shopKey;
            Category = category;
        }

        #endregion

        #region Methods

        public bool SaveDataBlankOnDesktop(string servicePath, string userPath, List<Product> selectedProducts,
            DateTime deliveryDate, bool isReturnProduct = false)
        {
            if (servicePath == string.Empty)
                throw new ArgumentException("Служебный путь сохранения бланка не может быть пустым.");


            XDocument document = new XDocument();

            // создадим корневой элемент
            XElement xRoot = new XElement("blank");
            // запишем атрибут - код магазина
            xRoot.Add(new XAttribute("keyShop", ShopKey));
            xRoot.Add(new XAttribute("isReturnProduct", isReturnProduct));
            xRoot.Add(new XAttribute("deliveryDate", deliveryDate));

            // добавим узел категории
            XElement categoryNode = new XElement("category");
            // запишем атрибуты id и имя категории
            categoryNode.Add(new XAttribute("id", Category.Id));
            categoryNode.Add(new XAttribute("name", Category.Name));

            // обойдем выбранные продукты и создадим для каждого элемент product и добавил его в узел категории
            foreach (Product item in selectedProducts)
            {
                XElement productNode = new XElement("product");

                productNode.Add(new XElement("id", item.Id));
                productNode.Add(new XElement("name", item.Name));
                productNode.Add(new XElement("count", item.Count));

                categoryNode.Add(productNode);
            }
            xRoot.Add(categoryNode);


            bool isSave = false;

            // Добавим корневой элемент в XML документ и сохраним его
            document.Add(xRoot);
            try
            {
                document.Save(servicePath);
                isSave = true;
            }
            catch (Exception)
            {
                return isSave;
            }

            if (userPath != string.Empty)
            {
                try
                {
                    document.Save(userPath);
                }
                catch (Exception)
                {

                }
            }

            return isSave;
        }

        #endregion
    }
}