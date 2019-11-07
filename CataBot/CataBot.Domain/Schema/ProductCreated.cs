using System;

namespace CataBot.Domain.Schema
{
    public class ProductCreated
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
    }
}
