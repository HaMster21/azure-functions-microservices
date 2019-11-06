using System;

namespace CataBot.Domain.Model
{
    public class Product
    {
        public Guid ID { get; set; }
        public long GTIN { get; set; }

        public string Name { get; set; }
        public string Manufacturer { get; set; }

        public int ECLASS { get; set; }
        public string Category { get; set; }
    }
}