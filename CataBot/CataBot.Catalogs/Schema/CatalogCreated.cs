using System;

namespace CataBot.Catalogs.Schema
{
    public class CatalogCreated
    {
        public Guid ID { get; }
        public string Name { get; }

        public CatalogCreated(Guid iD, string name)
        {
            ID = iD;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
