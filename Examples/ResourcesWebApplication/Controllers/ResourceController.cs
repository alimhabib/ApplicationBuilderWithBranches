using CommonServices;

using Microsoft.AspNetCore.Mvc;

namespace ResourcesWebApplication.Controllers
{
    [Route("[controller]")]
    public class ResourceController : ControllerBase
    {
        private IHiService _resourceService;

        public ResourceController(IHiService resourceService)
        {
            _resourceService = resourceService;
        }

        [HttpGet]
        public string Get()
        {
            return "I'm Resource Controller. " + _resourceService.SayHi();
        }
    }
}
