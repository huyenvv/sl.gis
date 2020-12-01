using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Model
{
    public class HydropowerViewModel
    {
        public List<string> SelectedHydropowerPlantOwners { get; set; } = new List<string>();

        public List<string> SelectedHydropowerDamsOwners { get; set; } = new List<string>();

        public List<string> SelectedConnections { get; set; } = new List<string>();

        public IFormFile HydropowerPlantImage { get; set; }
        public IFormFile HydropowerDamsImage { get; set; }
    }
}
