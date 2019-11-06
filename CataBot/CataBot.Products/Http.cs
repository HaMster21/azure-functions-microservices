using System;
using System.IO;
using System.Threading.Tasks;
using CataBot.Domain.Model;
using CataBot.Products.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CataBot.Products
{
    public static class Http
    {
        [FunctionName("NewProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "product")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating a new product");

            using (var reader = new StreamReader(req.Body))
            {
                var requestBody = await reader.ReadToEndAsync();
                dynamic newProductInput = JsonConvert.DeserializeObject(requestBody);

                var newProduct = new Product()
                {
                    ID = Guid.NewGuid(),
                    Name = newProductInput.Name,
                    GTIN = newProductInput.GTIN,
                    Category = newProductInput.Category ?? "none",
                    ECLASS = newProductInput.ECLASS ?? 0,
                    Manufacturer = newProductInput.Manufacturer ?? "Unknown"
                };

                using (var dbcontext = new ProductsContext(new DbContextOptionsBuilder<ProductsContext>().UseSqlServer(Environment.GetEnvironmentVariable("DbConnection")).Options))
                {
                    dbcontext.Products.Add(newProduct);
                    await dbcontext.SaveChangesAsync();
                }

                return new CreatedResult("", newProduct);
            }

        }
    }
}
