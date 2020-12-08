using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Map
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHydropowerPlantRepository _computerRepository;

        public IndexModel(ILogger<IndexModel> logger, IHydropowerPlantRepository computerRepository)
        {
            _logger = logger;
            _computerRepository = computerRepository;
        }

        public List<HydropowerPlant> HydropowerPlants { get; set; } = new List<HydropowerPlant>();

        public List<string> ElectricLevels = new List<string>();
        public void OnGet()
        {
            ElectricLevels = Core.Model.Substation.ElectricLevels;
            HydropowerPlants = _computerRepository.Find(m => true).ToList();
        }
    }
}
