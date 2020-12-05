﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Implementation;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SLGIS.Web.Areas.Admin.Pages.Element
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IElementRepository _elementRepository;

        public IndexModel(ILogger<IndexModel> logger, IElementRepository elementRepository, HydropowerService hydropowerService) : base(hydropowerService)
        {
            _logger = logger;
            _elementRepository = elementRepository;
        }

        public string FilterText { get; set; }
        public PagerViewModel ViewModel { get; set; }
        public SelectList HydropowerSelectList { get; set; }

        public void OnGet(string searchText = null, Guid? hydropowerId = null, int? pageIndex = 1)
        {
            HydropowerSelectList = CreateHydropowerPlantSelection(hydropowerId);
            FilterText = searchText;
            var list = _elementRepository.Find(m => true).AsQueryable();
            if (!string.IsNullOrEmpty(FilterText))
            {
                list = list.Where(m => m.Title.ToLower().Contains(FilterText.ToLower()));
            }

            if (hydropowerId.HasValue)
            {
                list = list.Where(m => m.HydropowerPlantId == hydropowerId);
            }

            var data = list.OrderByDescending(m => m.Created).AsEnumerable();
            var pager = new Pager(data.Count(), pageIndex);
            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index"),
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

            await _elementRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted element {id}");

            return RedirectToPage("./Index");
        }
    }
}
