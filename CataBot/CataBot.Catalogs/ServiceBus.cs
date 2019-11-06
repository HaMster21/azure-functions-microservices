using System;
using System.Collections.Generic;
using CataBot.Catalogs.Data;
using CataBot.Domain.Model;
using CataBot.Domain.Schema;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceBus.Messaging;

namespace CataBot.Catalogs
{
    public static class ServiceBus
    {
        [FunctionName("ImportNewProduct")]
        public static async void ImportProduct(
            [ServiceBusTrigger("product-created", Connection = "ServiceBusConnection")] BrokeredMessage incMessage)
        {
            var newProduct = incMessage.GetBody<ProductCreated>();

            using (var dbcontext = new CatalogContext(new DbContextOptionsBuilder<CatalogContext>().UseSqlServer(Environment.GetEnvironmentVariable("DbConnection")).Options))
            {
                var categoryCatalogFound = await dbcontext.Catalogs.AnyAsync(catalog => catalog.Name == newProduct.Category);

                if (!categoryCatalogFound)
                {
                    dbcontext.Catalogs.Add(new Catalog()
                    {
                        ID = Guid.NewGuid(),
                        Name = newProduct.Category,
                        Products = new List<Product>()
                    });
                    await dbcontext.SaveChangesAsync();
                }
            }
        }
    }
}
