using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Substation
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ISubstationRepository _substationRepository;

        public IndexModel(ILogger<IndexModel> logger, ISubstationRepository substationRepository)
        {
            _logger = logger;
            _substationRepository = substationRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }

        public void OnGet(string searchText = null, int? pageIndex = 1)
        {
            FilterText = searchText;
            Expression<Func<Core.Model.Substation, bool>> predicate = m => true;
            if (!string.IsNullOrEmpty(FilterText))
            {
                predicate = m => m.Name.ToLower().Contains(FilterText.ToLower());
            }

            var list = _substationRepository.Find(predicate).OrderByDescending(m => m.Created).AsEnumerable();

            var pager = new Pager(list.Count(), pageIndex);

            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index", new { searchText }),
                Items = list.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Page();
            }

            await _substationRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted substation {id}");

            return RedirectToPage("./Index");
        }

        public void Import()
        {
            var path = @"D:\Du an CNTT\110KV.csv";
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();
                var lst = new List<Core.Model.Substation>();
                var i = 0;
                var loo = string.Empty;
                var lastWard = string.Empty;
                var lastDictrict = string.Empty;
                while (!csvParser.EndOfData)
                {
                    ++i;
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    if (!int.TryParse(fields[0], out int _))
                    {
                        loo = fields[0];
                        continue;
                    }
                    if (!string.IsNullOrWhiteSpace(fields[3]))
                    {
                        lastWard = fields[3];
                    }
                    if (!string.IsNullOrWhiteSpace(fields[4]))
                    {
                        lastDictrict = fields[4];
                    }

                    var substation = new Core.Model.Substation();
                    substation.ElectricLevel = "110KV";
                    int.TryParse(fields[0].Trim(), out int number);
                    substation.ColumnNumber = number;
                    if (!double.TryParse(fields[1], out double _) || !double.TryParse(fields[2], out double _))
                        throw new Exception($"Dữ liệu không hợp lệ, tại dòng {i}");
                    substation.Location.Lat = fields[1].Trim();
                    substation.Location.Lng = fields[2].Trim();

                    substation.LineName = loo;
                    substation.Address = $"{lastWard}, {lastDictrict}";
                    substation.Ward = lastWard;
                    substation.Dictrict = lastDictrict;
                    lst.Add(substation);

                }
                _substationRepository.AddRangeAsync(lst);
            }
        }
    }
}
