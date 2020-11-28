using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLGIS.Core;
using SLGIS.Core.Repositories;
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
    public class SubstationController : ControllerBase
    {
        private readonly ISubstationRepository _substationRepository;
        public SubstationController(ISubstationRepository substationRepository)
        {
            _substationRepository = substationRepository;
        }

        [Route("map")]
        [HttpGet]
        public IEnumerable<SubstationModel> ListForMap()
        {
            return _substationRepository.Find(m => true).ToList().Select(m => new SubstationModel
            {
                Substation = m,
                CanManage = User.Identity.IsAuthenticated && (User.IsInRole(Constant.Role.Admin) || User.IsInRole(Constant.Role.Admin))
            });
        }
    }
}
