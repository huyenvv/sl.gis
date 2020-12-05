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
    public class NewOrEditDamsModel : PageModel
    {
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        private readonly ISubstationRepository _substationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileService _fileService;

        public NewOrEditDamsModel(IHydropowerPlantRepository hydropowerPlantRepository, ISubstationRepository substationRepository,
            IUserRepository userRepository, IFileService fileService)
        {
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _substationRepository = substationRepository;
            _userRepository = userRepository;
            _fileService = fileService;
        }

        [BindProperty]
        public string DamsLocation { get; set; }

        [BindProperty]
        public Guid PlantId { get; set; }

        [BindProperty]
        public HydropowerDams HydropowerDams { get; set; }

        [BindProperty]
        public HydropowerViewModel ViewModel { get; set; } = new HydropowerViewModel();

        public List<User> Members { get { return _userRepository.Find(m => m.Roles.Count == 0).ToList(); } }
        public List<Core.Model.Substation> Substations { get { return _substationRepository.Find(m => true).ToList(); } }

        public async Task<IActionResult> OnGetAsync(Guid plantId, Guid? id)
        {
            PlantId = plantId;

            if (id == null)
            {
                HydropowerDams = new HydropowerDams();
                return Page();
            }
            var plant = await _hydropowerPlantRepository.GetAsync(m => m.Id == plantId);
            HydropowerDams = plant.HydropowerDams.First(m => m.Id == id);

            ViewModel.SelectedHydropowerDamsOwners = HydropowerDams.Owners?.Select(m => m.ToString()).ToList();
            DamsLocation = HydropowerDams.Location.ToString();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HydropowerDams.Owners = ViewModel.SelectedHydropowerDamsOwners?.Select(m => new ObjectId(m)).ToList();
            if (DamsLocation != null && DamsLocation.Split(',').Length >= 2)
            {
                HydropowerDams.Location.Lat = DamsLocation.Split(',')[0].Trim();
                HydropowerDams.Location.Lng = DamsLocation.Split(',')[1].Trim();
            }
            await UpsertImage();

            var plant = await _hydropowerPlantRepository.GetAsync(PlantId);
            var listDams = plant.HydropowerDams;
            if (HydropowerDams.Id != Guid.Empty)
            {
                var dams = listDams.First(m => m.Id == HydropowerDams.Id);
                if (!Constant.Role.All.Any(User.IsInRole))
                {
                    HydropowerDams.Owners = dams.Owners;
                }
                listDams = listDams.Where(m => m.Id != HydropowerDams.Id).ToList();
            }
            else
            {
                HydropowerDams.Id = Guid.NewGuid();
            }

            listDams.Add(HydropowerDams);
            plant.HydropowerDams = listDams;
            await _hydropowerPlantRepository.UpsertAsync(plant);
            return RedirectToPage("./NewOrEdit", new { id = plant.Id });
        }

        private async Task UpsertImage()
        {
            if (ViewModel.HydropowerDamsImage != null)
            {
                var filePath = await _fileService.UpsertAsync(await ViewModel.HydropowerDamsImage.GetBytes(), ViewModel.HydropowerDamsImage.FileName, null, true);
                HydropowerDams.Image = filePath.Replace("wwwroot", "");
            }
        }
    }
}
