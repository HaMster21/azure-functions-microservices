using System;
using System.Collections.Generic;
using System.Text;

namespace CataBot.Domain.Model
{
    public class Catalog
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; }
    }
}
