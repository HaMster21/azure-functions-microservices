using System;
using System.Collections.Generic;
using System.Text;
using CataBot.Catalogs.Data;
using CataBot.Domain.Model;
using CataBot.Domain.Schema;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace CataBot.Catalogs
{
    public static class ServiceBus
    {
        [FunctionName("ImportNewProduct")]
        public static async void ImportProduct(
            [ServiceBusTrigger("product-created", Connection = "ServiceBusConnection")] BrokeredMessage incMessage,
            [ServiceBusTrigger("catalog-create-command", Connection = "ServiceBusConnection")] IAsyncCollector<Message> newCatalogQueue,
            ILogger log)
        {
            var newProduct = incMessage.GetBody<ProductCreated>();

            log.LogInformation($"Importing product");

            using (var dbcontext = new CatalogContext(new DbContextOptionsBuilder<CatalogContext>().UseSqlServer(Environment.GetEnvironmentVariable("DbConnection")).Options))
            {
                var categoryCatalogFound = await dbcontext.Catalogs.AnyAsync(catalog => catalog.Name == newProduct.Category);

                if (!categoryCatalogFound)
                {
                    await newCatalogQueue.AddAsync(new Message()
                    {
                        CorrelationId = incMessage.CorrelationId,
                        Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Catalog()
                        {
                            ID = Guid.NewGuid(),
                            Name = newProduct.Category,
                            Description = string.Empty,
                            Products = new List<Product>()
                        }))
                    });
                }
            }
        }

        [FunctionName("CreateCatalog")]
        public static async void CreateCatalog(
            [ServiceBusTrigger("catalog-create-command", Connection = "ServiceBusConnection")] BrokeredMessage incMessage,
            ILogger log)
        {
            var newCatalog = incMessage.GetBody<Catalog>();
            log.LogInformation($"Creating new catalog {newCatalog.Name}");

            using (var dbcontext = new CatalogContext(new DbContextOptionsBuilder<CatalogContext>().UseSqlServer(Environment.GetEnvironmentVariable("DbConnection")).Options))
            {
                dbcontext.Catalogs.Add(newCatalog);
                await dbcontext.SaveChangesAsync();
            }
        }
    }
}
