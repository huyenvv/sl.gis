using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Core.Extension;
using SLGIS.Implementation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace SLGIS.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostDataController : ControllerBase
    {
        private readonly IElementRepository _elementRepository;
        private readonly IPostDataRepository _postDataRepository;
        private readonly HydropowerService _hydropowerService;
        private readonly int _pageSize = 100;
        public PostDataController(IElementRepository elementRepository, IPostDataRepository postDataRepository, HydropowerService hydropowerService)
        {
            _elementRepository = elementRepository;
            _postDataRepository = postDataRepository;
            _hydropowerService = hydropowerService;
        }

        [HttpGet("{hydropowerPlantId}")]
        public ActionResult<IEnumerable<dynamic>> GetDataForGraph(Guid hydropowerPlantId, string start, string end)
        {
            if (string.IsNullOrEmpty(start)
                || !DateTime.TryParseExact(start, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                startDate = DateTime.Now.Date;
            }

            if (string.IsNullOrEmpty(end)
                || !DateTime.TryParseExact($"{end} 23:59:59", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                endDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }

            startDate = startDate.ToVNDate();
            endDate = endDate.ToVNDate();

            var items = _elementRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId).Select(m => new { Id = m.Code, m.Title, m.Unit }).ToList();
            var itemValues = _postDataRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId
                                        && m.Date >= startDate.ToVNDate() && m.Date <= endDate.ToVNDate()).ToList();
            var data = items.Select(m =>
            {
                var values = itemValues.SelectMany(x => x.PostDataDetails
                            .SelectMany(t => t.Values.Where(k => k.Code == m.Id).Select(z => new { t.Time, z.Value, Id = z.Code })));
                return new
                {
                    Item = m,
                    Data = values
                };
            }).ToList();

            return data;
        }

        /// <summary>
        /// Get detail data for bar chart
        /// </summary>
        /// <param name="hydropowerPlantId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet("{hydropowerPlantId}/detail")]
        public ActionResult<IEnumerable<dynamic>> GetDataDetail(Guid hydropowerPlantId, int? year)
        {
            var now = DateTime.UtcNow.AddHours(7);
            var toMonth = 12;
            if (!year.HasValue)
            {
                year = now.Year;
                toMonth = now.Month;
            }

            List<double> sanluongNgay, soGioPhatDien, totalWater;
            sanluongNgay = new List<double>();
            soGioPhatDien = new List<double>();
            totalWater = new List<double>();

            var itemValues = _postDataRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId);
            for (var month = 1; month <= toMonth; month++)
            {
                var firstOfMonth = new DateTime(year.Value, month, 1);
                var endOfMonth = firstOfMonth.AddMonths(1).AddTicks(-1);

                var monthValue = itemValues.Where(m => m.Date >= firstOfMonth.ToVNDate() && m.Date <= endOfMonth.ToVNDate());
                sanluongNgay.Add(monthValue.Sum(m => m.SanLuongNgay));
                totalWater.Add(monthValue.Sum(m => m.TotalWater));
                soGioPhatDien.Add(monthValue.Sum(m => m.SoGioPhatDien));
            }

            return new List<List<double>> { sanluongNgay, totalWater, soGioPhatDien };
        }

        [HttpGet("csv-average")]
        public ActionResult<dynamic> GetCsvAverageData(Guid? hydropowerPlantId, string start, string end, int page = 1)
        {
            var data = GetCsvData(hydropowerPlantId, start, end, page);
            var plants = _hydropowerService.CurrentList(User.GetId());
            var result = data.Item2.Select(x =>
                new { x.Date, x.SanLuongNgay, x.SoGioPhatDien, x.TotalWater, x.HydropowerPlantId })
                .ToList();

            return result
                .Select(x => new { x.Date, x.SanLuongNgay, x.SoGioPhatDien, x.TotalWater, PlantName = plants.FirstOrDefault(m => m.Id == x.HydropowerPlantId).Name })
                .ToList();
        }

        [HttpGet("csv-detail")]
        public ActionResult<dynamic> GetCsvDataDetail(Guid? hydropowerPlantId, string start, string end, int page = 1)
        {
            var data = GetCsvData(hydropowerPlantId, start, end, page);
            var plants = _hydropowerService.CurrentList(User.GetId());
            var result = data.Item2.Select(x => new { x.HydropowerPlantId, x.PostDataDetails }).ToList();

            return new
            {
                Items = data.Item1,
                Data = result.Select(x => new { PlantName = plants.FirstOrDefault(m => m.Id == x.HydropowerPlantId).Name, x.PostDataDetails }).ToList()
            };
        }

        private (dynamic, IQueryable<PostData>) GetCsvData(Guid? hydropowerPlantId, string start, string end, int page = 1)
        {
            if (string.IsNullOrEmpty(start)
                || !DateTime.TryParseExact(start, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                startDate = DateTime.Now.Date;
            }

            if (string.IsNullOrEmpty(end)
                || !DateTime.TryParseExact($"{end} 23:59:59", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                endDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }

            startDate = startDate.ToVNDate();
            endDate = endDate.ToVNDate();

            var isAdmin = Constant.Role.All.Any(User.IsInRole);
            if (!isAdmin)
            {
                hydropowerPlantId = _hydropowerService.GetCurrent(User.GetId()).Id;
            }

            var items = _elementRepository.Find(m => true).AsQueryable();
            var values = _postDataRepository.Find(m => true).AsQueryable();
            if (hydropowerPlantId.HasValue)
            {
                items = items.Where(m => m.HydropowerPlantId == hydropowerPlantId);
                values = values.Where(m => m.HydropowerPlantId == hydropowerPlantId);
            }
            else
            {
                items = items.Where(m => !m.HydropowerPlantId.HasValue);
            }

            var skip = (page <= 1 ? 0 : page - 1) * _pageSize;
            values = values.Where(m => m.Date >= startDate && m.Date <= endDate).Skip(skip).Take(_pageSize);

            return (items.Select(m => new { Id = m.Code, m.Title, m.Unit }).ToList(), values);
        }
    }
}
