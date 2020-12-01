using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Core.Model.ValueObjects;
using SLGIS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Hydropower
{
    public class NewOrEditModel : PageModel
    {
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        private readonly ISubstationRepository _substationRepository;
        private readonly IUserRepository _userRepository;

        public NewOrEditModel(IHydropowerPlantRepository hydropowerPlantRepository, ISubstationRepository substationRepository, IUserRepository userRepository)
        {
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _substationRepository = substationRepository;
            _userRepository = userRepository;
        }

        [BindProperty]
        public string PlantLocation { get; set; }

        [BindProperty]
        public string DamsLocation { get; set; }

        [BindProperty]
        public Core.HydropowerPlant HydropowerPlant { get; set; }

        [BindProperty]
        public List<string> SelectedHydropowerPlantOwners { get; set; } = new List<string>();

        [BindProperty]
        public List<string> SelectedHydropowerDamsOwners { get; set; } = new List<string>();

        [BindProperty]
        public List<string> SelectedConnections { get; set; } = new List<string>();

        public List<User> Members { get { return _userRepository.Find(m => m.Roles.Count == 0).ToList(); } }
        public List<Core.Model.Substation> Substations { get { return _substationRepository.Find(m => true).ToList(); } }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                HydropowerPlant = new Core.HydropowerPlant();
                return Page();
            }

            HydropowerPlant = await _hydropowerPlantRepository.GetAsync(m => m.Id == id);

            if (HydropowerPlant == null)
            {
                return NotFound();
            }

            SelectedHydropowerPlantOwners = HydropowerPlant.Owners.Select(m => m.ToString()).ToList();
            SelectedHydropowerDamsOwners = HydropowerPlant.HydropowerDams?.Owners?.Select(m => m.ToString()).ToList();
            SelectedConnections = HydropowerPlant.Connections?.Select(m => m.SubstationId.ToString()).ToList();
            PlantLocation = HydropowerPlant.Location.ToString();
            DamsLocation = HydropowerPlant.HydropowerDams.Location.ToString();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HydropowerPlant.Owners = SelectedHydropowerPlantOwners?.Select(m => new ObjectId(m)).ToList();
            HydropowerPlant.HydropowerDams.Owners = SelectedHydropowerDamsOwners?.Select(m => new ObjectId(m)).ToList();
            HydropowerPlant.Connections = SelectedConnections?.Select(m => new Connection { SubstationId = Guid.Parse(m) }).ToList();

            if (PlantLocation != null && PlantLocation.Split(',').Length >= 2)
            {
                HydropowerPlant.Location.Lat = PlantLocation.Split(',')[0].Trim();
                HydropowerPlant.Location.Lng = PlantLocation.Split(',')[1].Trim();
            }

            if (DamsLocation != null && DamsLocation.Split(',').Length >= 2)
            {
                HydropowerPlant.HydropowerDams.Location.Lat = DamsLocation.Split(',')[0].Trim();
                HydropowerPlant.HydropowerDams.Location.Lng = DamsLocation.Split(',')[1].Trim();
            }

            await _hydropowerPlantRepository.UpsertAsync(HydropowerPlant);

            return RedirectToPage("./Index");
        }
    }
}
