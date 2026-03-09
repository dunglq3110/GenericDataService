using GenericDataService.Contract;
using GenericDataService.Production.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GenericDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductionServiceFactory _serviceFactory; // list of "Generic service here"

        public ProductController(ProductionServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        //private readonly IExecutionContextAccessor _executionContext;
        //private readonly IEnvironmentContext _environment; // abstracted, as discussed

        [HttpPost("{signature}/{functionCode}")]
        public async Task<IActionResult> Execute(
            string signature,
            string functionCode,
            [FromBody] dynamic payload) // just the data body, no signature/functionCode in body
        {
            try
            {
                //get the user id by bearer token jwt, from now just hardcoded "admin" somewhere that convenient to get
                var service = _serviceFactory.GetService(signature);
                var result = await service.HandleAsync(functionCode, payload);
                return result.Success ? Ok(result.Data) : BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel(ex));
            }
        }
    }
}
