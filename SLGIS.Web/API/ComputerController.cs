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
        private readonly IComputerRepository _computerRepository;
        public ComputerController(IComputerRepository computerRepository)
        {
            _computerRepository = computerRepository;
        }

        [HttpGet()]
        public IEnumerable<Computer> Index()
        {
            return _computerRepository.Find(m => true);
        }
    }
}
