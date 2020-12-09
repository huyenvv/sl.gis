using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class NewOrEditModel : PageModelBase
    {
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        private readonly ISubstationRepository _substationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IElementRepository _elementRepository;
        private readonly IFileService _fileService;
        private readonly HydropowerService _hydropowerService;

        public NewOrEditModel(IHydropowerPlantRepository hydropowerPlantRepository, ISubstationRepository substationRepository,
            IUserRepository userRepository, IElementRepository elementRepository, IFileService fileService, HydropowerService hydropowerService)
            : base(hydropowerService)
        {
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _substationRepository = substationRepository;
            _userRepository = userRepository;
            _elementRepository = elementRepository;
            _fileService = fileService;
            _hydropowerService = hydropowerService;
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
                if (!CanManage)
                {
                    return RedirectToPage("./Index");
                }

                HydropowerPlant = new Core.HydropowerPlant();
                return Page();
            }

            HydropowerPlant = await _hydropowerPlantRepository.GetAsync(m => m.Id == id);

            if (HydropowerPlant == null)
            {
                return NotFound();
            }

            ViewModel.SelectedHydropowerPlantOwners = HydropowerPlant.Owners.Select(m => m.ToString()).ToList();
            ViewModel.SelectedConnections = HydropowerPlant.Connections?.Select(m => m.SubstationId.ToString()).ToList();
            PlantLocation = HydropowerPlant.Location.ToString();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HydropowerPlant.Owners = ViewModel.SelectedHydropowerPlantOwners?.Select(m => new ObjectId(m)).ToList();
            HydropowerPlant.Connections = ViewModel.SelectedConnections?.Select(m => new Connection { SubstationId = Guid.Parse(m) }).ToList();
            if (PlantLocation != null && PlantLocation.Split(',').Length >= 2)
            {
                HydropowerPlant.Location.Lat = PlantLocation.Split(',')[0].Trim();
                HydropowerPlant.Location.Lng = PlantLocation.Split(',')[1].Trim();
            }
            await UpsertImage();
            var isCreate = false;
            if (HydropowerPlant.Id != Guid.Empty)
            {
                var plant = await _hydropowerPlantRepository.GetAsync(HydropowerPlant.Id);
                HydropowerPlant.HydropowerDams = plant.HydropowerDams;

                if (CanManage)
                {
                    var removeUserIds = plant.Owners?.Where(m => HydropowerPlant.Owners?.Any(n => n == m) == false).ToArray();
                    _hydropowerService.RemoveCache(removeUserIds);
                }
                else
                {
                    HydropowerPlant.Connections = plant.Connections;
                    HydropowerPlant.Owners = plant.Owners;
                }
            }
            else
            {
                isCreate = true;
                HydropowerPlant.Owners.Add(User.GetId());
            }

            var result = await _hydropowerPlantRepository.UpsertAsync(HydropowerPlant);

            await InsertSampleElements(isCreate, result.Id);
            return RedirectToPage("./Index");
        }

        private async Task InsertSampleElements(bool isCreate, Guid plantId)
        {
            if (!isCreate)
            {
                return;
            }

            var samples = _elementRepository.Find(m => !m.HydropowerPlantId.HasValue).ToList();
            foreach (var item in samples)
            {
                item.Id = Guid.Empty;
                item.HydropowerPlantId = plantId;
            }

            await _elementRepository.AddRangeAsync(samples);
        }

        public async Task<IActionResult> OnPostDeleteDamsAsync(Guid plantId, Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToPage("./NewOrEdit", new { id = plantId });
            }

            var plant = await _hydropowerPlantRepository.GetAsync(plantId);
            plant.HydropowerDams = plant.HydropowerDams.Where(m => m.Id != id).ToList();

            await _hydropowerPlantRepository.UpsertAsync(plant);
            return RedirectToPage("./NewOrEdit", new { id = plantId });
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
                //HydropowerPlant.HydropowerDams.Image = filePath.Replace("wwwroot", "");
            }
        }
    }
}
