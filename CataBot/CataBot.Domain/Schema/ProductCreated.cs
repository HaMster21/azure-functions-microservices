using System;

namespace CataBot.Domain.Schema
{
    public class ProductCreated
    {
        public Guid ID { get; }
        public string Name { get; }
        public string Category { get; }

        public ProductCreated(Guid iD, string name, string category)
        {
            ID = iD;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Category = category ?? throw new ArgumentNullException(nameof(category));
        }
    }
}
