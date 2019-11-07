using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CataBot.Catalogs.Data;
using CataBot.Catalogs.Schema;
using CataBot.Domain.Model;
using CataBot.Domain.Schema;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CataBot.Catalogs
{
    public static class ServiceBus
    {
        [FunctionName("ImportNewProduct")]
        public static async Task ImportProduct(
            [ServiceBusTrigger("product-created", "catalog-service", Connection = "ServiceBusConnection")] Message incMessage,
            [ServiceBus("catalog-create-command", Connection = "ServiceBusConnection")] IAsyncCollector<Message> newCatalogQueue,
            ILogger log)
        {
            var newProduct = JsonConvert.DeserializeObject<ProductCreated>(Encoding.UTF8.GetString(incMessage.Body));

            using (var dbcontext = new CatalogContext(new DbContextOptionsBuilder<CatalogContext>().UseSqlServer(Environment.GetEnvironmentVariable("DbConnection")).Options))
            {
                var categoryCatalogFound = await dbcontext.Catalogs.AnyAsync(catalog => catalog.Name == newProduct.Category);

                if (!categoryCatalogFound)
                {
                    log.LogInformation($"Creating new catalog for category {newProduct.Category} for product {newProduct.ID} ({newProduct.Name})");

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
            };
        }

        [FunctionName("CreateCatalog")]
        public static async Task CreateCatalog(
            [ServiceBusTrigger("catalog-create-command", Connection = "ServiceBusConnection")] Message commandMessage,
            [ServiceBus("catalog-created", Connection = "ServiceBusConnection")] IAsyncCollector<Message> newCatalogTopic,
            ILogger log)
        {
            var newCatalog = JsonConvert.DeserializeObject<Catalog>(Encoding.UTF8.GetString(commandMessage.Body));

            using (var dbcontext = new CatalogContext(new DbContextOptionsBuilder<CatalogContext>().UseSqlServer(Environment.GetEnvironmentVariable("DbConnection")).Options))
            {
                dbcontext.Catalogs.Add(newCatalog);
                await dbcontext.SaveChangesAsync();
            }

            await newCatalogTopic.AddAsync(
                new Message()
                {
                    CorrelationId = commandMessage.CorrelationId,
                    Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new CatalogCreated(newCatalog.ID, newCatalog.Name)))
                });

        }
    }
}
