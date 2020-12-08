using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SLGIS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace SLGIS.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostDataController : ControllerBase
    {
        private readonly IElementRepository _elementRepository;
        private readonly IPostDataRepository _postDataRepository;
        public PostDataController(IElementRepository elementRepository, IPostDataRepository postDataRepository)
        {
            _elementRepository = elementRepository;
            _postDataRepository = postDataRepository;
        }

        [HttpGet("{hydropowerPlantId}")]
        public ActionResult<IEnumerable<dynamic>> GetDataForGraph(Guid hydropowerPlantId, string start, string end)
        {
            if (string.IsNullOrEmpty(start)
                || !DateTime.TryParseExact(start, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                startDate = DateTime.UtcNow.Date;
            }

            if (string.IsNullOrEmpty(end)
                || !DateTime.TryParseExact($"{end} 23:59:59", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                endDate = DateTime.UtcNow.Date;
            }

            var items = _elementRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId).Select(m => new { Id = m.Code, m.Title, m.Unit }).ToList();
            var itemValues = _postDataRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId && m.Date >= startDate && m.Date <= endDate).ToList();
            var data = items.Select(m =>
            {
                var values = itemValues.SelectMany(x => x.PostDataDetails.SelectMany(t => t.Values.Where(k => k.Code == m.Id).Select(z => new { t.Time, z.Value, Id = z.Code })));
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
            var itemValues = _postDataRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId);
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

            for (var month = 1; month <= toMonth; month++)
            {
                var dt = new DateTime(year.Value, month, 1);
                var firstOfMonth = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                var endOfMonth = firstOfMonth.AddMonths(1).AddTicks(-1);

                var monthValue = itemValues.Where(m => m.Date >= firstOfMonth && m.Date <= endOfMonth).ToList();
                sanluongNgay.Add(monthValue.Sum(m => m.SanLuongNgay));
                totalWater.Add(monthValue.Sum(m => m.TotalWater));
                soGioPhatDien.Add(monthValue.Sum(m => m.SoGioPhatDien));
            }

            return new List<List<double>> { sanluongNgay, totalWater, soGioPhatDien };
        }
    }
}
