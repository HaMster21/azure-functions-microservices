using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CataBot.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CataBot.Catalogs
{
    public static class Http
    {
        [FunctionName("RequestCreateCatalog")]
        public static async Task<IActionResult> RequestCreateCatalog(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "catalog")] HttpRequest req,
            [ServiceBus("catalog-create-command", Connection = "ServiceBusConnection")] IAsyncCollector<Message> newCatalogQueue,
            ILogger log)
        {
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

                await newCatalogQueue.AddAsync(new Message()
                {
                    CorrelationId = req.Headers["CorrelationID"],
                    Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(newCatalog))
                });

                return new AcceptedResult();
            }
        }
    }
}
