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
                CanManage = User.Identity.IsAuthenticated && (User.IsInRole(Constant.Role.Admin) || User.IsInRole(Constant.Role.SupperAdmin) || m.Owners.Any(x => x == User.GetId()))
            });
        }

        [Route("change-data")]
        [HttpGet]
        public void ChangeData()
        {
            var plants = _hydropowerPlantRepository.Find(m => true).ToList();
            foreach (var item in plants)
            {
                if (item.HydropowerDams[0].Location != null && item.HydropowerDams[0].Location.Lat != "0")
                {
                    var lat = double.Parse(item.HydropowerDams[0].Location.Lat);
                    var lng = double.Parse(item.HydropowerDams[0].Location.Lng);
                    lat -= 0.00008167;
                    item.HydropowerDams[0].Location.Lat = lat + "";
                    lng -= 3.7499242;
                    item.HydropowerDams[0].Location.Lng = lng + "";
                    _hydropowerPlantRepository.UpdateAsync(item);
                }
            }
        }
    }
}
