using GenericDataService.EntityModel.Production;
using GenericDataService.GenericService;
using GenericDataService.GenericService.Attributes;
using GenericDataService.GenericService.Models;
using GenericDataService.Infrastructure.Pipeline;
using GenericDataService.Production.Infrastructure;

namespace GenericDataService.Production.Services.Services
{

    [ServiceSignature("CommonCodeMaster")]
    public class CommonCodeMasterService : GenericService<CommonCodeMaster>
    {
        public CommonCodeMasterService(ProductionDbContext db, ServicePipeline pipeline) : base(db, pipeline) { }

    }
}
