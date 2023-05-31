using System;

namespace ButcherShopLibrary
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Count { get; set; }
        public double Price { get; set; }
        public double Sum { get; set; }
    }
}