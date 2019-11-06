using System;

namespace Model
{
    public class Product
    {
        public Product()
        {
        }

        public Guid ID { get; set; }
        public long GTIN { get; set; }

        public string Name { get; set; }
        public string Manufacturer { get; set; }

        public int ECLASS { get; set; }
        public string Category { get; set; }
    }
}