using Microsoft.AspNetCore.Mvc;
using SLGIS.Core;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace SLGIS.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComputerController : ControllerBase
    {
        private readonly IFactoryRepository _computerRepository;
        public ComputerController(IFactoryRepository computerRepository)
        {
            _computerRepository = computerRepository;
        }

        [HttpGet()]
        public IEnumerable<Factory> Index()
        {
            return _computerRepository.Find(m => true);
        }
    }
}
