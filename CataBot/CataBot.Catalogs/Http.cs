using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CataBot.Catalogs.Data;
using CataBot.Catalogs.Schema;
using CataBot.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CataBot.Catalogs
{
    public static class Http
    {
        [FunctionName("CreateCatalog")]
        public static async Task<IActionResult> CreateCatalog(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "catalog")] HttpRequest req,
            [ServiceBus("catalog-created", Connection = "ServiceBusConnection")] IAsyncCollector<CatalogCreated> catalogCreatedTopic,
            ILogger log)
        {
            log.LogInformation("Creating a new catalog");

            using (var reader = new StreamReader(req.Body))
            {
                var requestBody = await reader.ReadToEndAsync();
                dynamic newCatalogInput = JsonConvert.DeserializeObject(requestBody);

                var newCatalog = new Catalog()
                {
                    ID = Guid.NewGuid(),
                    Name = newCatalogInput.Name,
                    Description = newCatalogInput.Description,
                    Products = new List<Product>()
                };

                using (var dbcontext = new CatalogContext(new DbContextOptionsBuilder<CatalogContext>().UseSqlServer(Environment.GetEnvironmentVariable("DbConnection")).Options))
                {
                    dbcontext.Catalogs.Add(newCatalog);
                    await dbcontext.SaveChangesAsync();
                }

                await catalogCreatedTopic.AddAsync(new CatalogCreated(newCatalog.ID, newCatalog.Name));

                return new CreatedResult("", newCatalog);
            }
        }
    }
}
