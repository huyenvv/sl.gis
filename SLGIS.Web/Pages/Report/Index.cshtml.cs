﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using SLGIS.Implementation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web.Pages.Report
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IReportRepository _reportRepository;
        private readonly IFileService _fileService;

        public IndexModel(ILogger<IndexModel> logger, IReportRepository reportRepository, IFileService fileService, HydropowerService hydropowerService) : base(hydropowerService)
        {
            _logger = logger;
            _reportRepository = reportRepository;
            _fileService = fileService;
        }

        [BindProperty]
        public SearchModel SearchModel { get; set; } = new SearchModel();
        public PagerViewModel ViewModel { get; set; }

        public IActionResult OnGet(SearchModel searchModel, int? pageIndex = 1)
        {
            if (!HasHydropower)
            {
                return ReturnToMap();
            }

            if (searchModel != null)
                SearchModel = searchModel;

            var hydropowerPlantId = GetCurrentHydropower().Id;
            ViewData["HydropowerPlantId"] = hydropowerPlantId;

            var list = _reportRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId).AsQueryable();
            if (!string.IsNullOrEmpty(SearchModel.FilterText))
            {
                list = list.Where(m => m.Title.Contains(SearchModel.FilterText.ToLower()));
            }

            if (SearchModel.StartDate.HasValue)
            {
                list = list.Where(m => m.Created >= SearchModel.StartDate);
            }

            if (SearchModel.EndDate.HasValue)
            {
                SearchModel.EndDate = SearchModel.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                list = list.Where(m => m.Created <= SearchModel.EndDate);
            }

            var result = list.OrderByDescending(m => m.Created).AsEnumerable();
            var pager = new Pager(list.Count(), pageIndex);
            ViewModel = new PagerViewModel
            {
                BaseUrl = Url.Page("./Index"),
                Items = list.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList(),
                Pager = pager
            };

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadFileAsync(Guid id, string fileName)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var report = await _reportRepository.GetAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            var filePath = report.Files.FirstOrDefault(m => System.IO.Path.GetFileName(m) == fileName);
            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound();
            }

            if (!System.IO.File.Exists(filePath))
                return Content("filename not present");

            var memory = await _fileService.GetAsync(filePath);
            return File(memory, Common.GetContentType(filePath), Path.GetFileName(filePath));
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Page();
            }

            await _reportRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted report {id}");

            return RedirectToPage("./Index");
        }
    }
}
