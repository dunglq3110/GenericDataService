using GenericDataService.EntityModel.Production;
using GenericDataService.GenericService.Attributes;
using GenericDataService.GenericService.Models;
using GenericDataService.Infrastructure;
using GenericDataService.Infrastructure.Pipeline;
using GenericDataService.Production.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenericDataService.Production.Services.Services
{
    [ServiceSignature("product")]
    public class ProductService : GenericService<Product>
    {
        public ProductService(ProductionDbContext db, ServicePipeline pipeline) : base(db, pipeline) { }

        [FunctionCode("Restock")]
        public async Task<ServiceResult> HandleRestock(JsonElement payload)
        {
            var productId = payload.GetProperty("productId").GetInt32();

            await Task.Delay(2000);

            return ServiceResult.Ok("Restocked.");
        }
    }
}
