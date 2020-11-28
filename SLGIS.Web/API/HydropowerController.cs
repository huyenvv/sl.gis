using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLGIS.Core;
using SLGIS.Web.Model.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class HydropowerController : ControllerBase
    {
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        public HydropowerController(IHydropowerPlantRepository hydropowerPlantRepository)
        {
            _hydropowerPlantRepository = hydropowerPlantRepository;
        }

        [Route("map")]
        [HttpGet]
        public IEnumerable<HydroPowerModel> ListForMap()
        {
            return _hydropowerPlantRepository.Find(m => true).ToList().Select(m => new HydroPowerModel
            {
                HydropowerPlant = m,
                CanManage = User.Identity.IsAuthenticated && (User.IsInRole(Constant.Role.Admin) || User.IsInRole(Constant.Role.Admin) || m.Owners.Any(x => x == User.GetId()))
            });
        }
    }
}
