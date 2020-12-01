using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Core.Model.ValueObjects;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
using SLGIS.Web.Model;
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
        private readonly IFileService _fileService;

        public NewOrEditModel(IHydropowerPlantRepository hydropowerPlantRepository, ISubstationRepository substationRepository,
            IUserRepository userRepository, IFileService fileService)
        {
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _substationRepository = substationRepository;
            _userRepository = userRepository;
            _fileService = fileService;
        }

        [BindProperty]
        public string PlantLocation { get; set; }

        [BindProperty]
        public string DamsLocation { get; set; }

        [BindProperty]
        public Core.HydropowerPlant HydropowerPlant { get; set; }

        [BindProperty]
        public HydropowerViewModel ViewModel { get; set; } = new HydropowerViewModel();

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

            ViewModel.SelectedHydropowerPlantOwners = HydropowerPlant.Owners.Select(m => m.ToString()).ToList();
            ViewModel.SelectedHydropowerDamsOwners = HydropowerPlant.HydropowerDams?.Owners?.Select(m => m.ToString()).ToList();
            ViewModel.SelectedConnections = HydropowerPlant.Connections?.Select(m => m.SubstationId.ToString()).ToList();
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

            HydropowerPlant.Owners = ViewModel.SelectedHydropowerPlantOwners?.Select(m => new ObjectId(m)).ToList();
            HydropowerPlant.HydropowerDams.Owners = ViewModel.SelectedHydropowerDamsOwners?.Select(m => new ObjectId(m)).ToList();
            HydropowerPlant.Connections = ViewModel.SelectedConnections?.Select(m => new Connection { SubstationId = Guid.Parse(m) }).ToList();
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
            await UpsertImage();
            await _hydropowerPlantRepository.UpsertAsync(HydropowerPlant);

            return RedirectToPage("./Index");
        }

        private async Task UpsertImage()
        {
            if (ViewModel.HydropowerPlantImage != null)
            {
                var filePath = await _fileService.UpsertAsync(await ViewModel.HydropowerPlantImage.GetBytes(), ViewModel.HydropowerPlantImage.FileName, null, true);
                HydropowerPlant.Image = filePath.Replace("wwwroot", "");
            }

            if (ViewModel.HydropowerDamsImage != null)
            {
                var filePath = await _fileService.UpsertAsync(await ViewModel.HydropowerDamsImage.GetBytes(), ViewModel.HydropowerDamsImage.FileName, null, true);
                HydropowerPlant.HydropowerDams.Image = filePath.Replace("wwwroot", "");
            }
        }
    }
}
