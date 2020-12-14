using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using SLGIS.Core;
using SLGIS.Core.Model.ValueObjects;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Hydropower
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        private readonly IElementRepository _elementRepository;

        public IndexModel(ILogger<IndexModel> logger, IHydropowerPlantRepository hydropowerPlantRepository, HydropowerService hydropowerService
            , IElementRepository elementRepository)
            : base(hydropowerService)
        {
            _logger = logger;
            _hydropowerPlantRepository = hydropowerPlantRepository;
            _elementRepository = elementRepository;
        }

        public string FilterText { get; set; }
        public string District { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public void OnGet(string searchText = null, string district = null, int? pageIndex = 1)
        {
            FilterText = searchText;
            District = district;
            var plants = _hydropowerPlantRepository.Find(m => true).AsQueryable();
            if (!string.IsNullOrEmpty(FilterText))
            {
                plants = plants.Where(m => m.Name.ToLower().Contains(FilterText.ToLower()));
            }

            if (!string.IsNullOrEmpty(district))
            {
                plants = plants.Where(m => m.District.Equals(district));
            }

            if (!CanManage)
            {
                plants = plants.Where(m => m.Owners.Contains(User.GetId()));
            }

            var data = plants.OrderBy(m => m.District).ThenByDescending(m => m.Created).ToList();

            var pager = new Pager(data.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index", new { searchText, district }),
                Items = data.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Page();
            }

            var plant = await _hydropowerPlantRepository.GetAsync(id);
            if (!CanManage && !plant.Owners.Any(m => m == User.GetId()))
            {
                return BadRequest();
            }

            await _hydropowerPlantRepository.DeleteAsync(id);
            var elements = _elementRepository.Find(m => m.HydropowerPlantId == plant.Id).ToList();
            foreach (var item in elements)
            {
                await _elementRepository.DeleteAsync(item.Id);
            }
            _logger.LogInformation($"Deleted hydropowerPlant {id}");

            return RedirectToPage("./Index");
        }

        public void Import()
        {
            var path = @"wwwroot\plant.csv";
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();
                var lst = new List<HydropowerPlant>();
                var i = 0;
                while (!csvParser.EndOfData)
                {
                    ++i;
                    try
                    {
                        // Read current line fields, pointer moves to the next line.
                        string[] fields = csvParser.ReadFields();
                        var plant = new HydropowerPlant();
                        plant.Name = fields[0];
                        plant.Address = fields[1];
                        plant.OwnerName = fields[2];
                        plant.Wattage = fields[3];
                        if (!string.IsNullOrWhiteSpace(fields[5]))
                        {
                            var valid = decimal.TryParse(fields[5], out decimal total);
                            if (!valid) throw new Exception();
                            plant.TotalInvestment = total;
                        }

                        if (!string.IsNullOrWhiteSpace(fields[6]))
                        {
                            var value = fields[6];
                            if (value.Contains("/")) value = value.Split('/')[1];
                            plant.EndBuild = int.Parse(value);
                        }

                        plant.Status = fields[7];
                        if (!string.IsNullOrWhiteSpace(fields[8]) && !string.IsNullOrWhiteSpace(fields[9]))
                        {
                            plant.Location.Lat = fields[8].Trim();
                            plant.Location.Lng = fields[9].Trim();
                        }

                        var dams = new HydropowerDams { Id = Guid.NewGuid(), Name = "Đập 1" };
                        plant.HydropowerDams = new List<HydropowerDams> { dams };

                        if (!string.IsNullOrWhiteSpace(fields[10]) && !string.IsNullOrWhiteSpace(fields[11]))
                        {
                            dams.Location.Lat = fields[10].Trim();
                            dams.Location.Lng = fields[11].Trim();
                        }

                        lst.Add(plant);
                    }
                    catch { throw new Exception($"Dữ liệu không hợp lệ, tại dòng {i}"); }
                }
                _hydropowerPlantRepository.AddRangeAsync(lst);
            }
        }

        public async Task UpdateDistricts()
        {
            var plants = _hydropowerPlantRepository.Find(m => true);
            foreach (var item in plants)
            {
                item.District = HydropowerPlant.Districts.FirstOrDefault(m => item.Address.Contains(m, StringComparison.OrdinalIgnoreCase));
                await _hydropowerPlantRepository.UpdateAsync(item);
            }
        }
    }
}
