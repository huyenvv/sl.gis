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

            var items = _elementRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId).Select(m=> new { Id = m.Code, m.Title}).ToList();
            var itemValues = _postDataRepository.Find(m => m.HydropowerPlantId == hydropowerPlantId && m.Date >= startDate && m.Date <= endDate).ToList();
            var data = items.Select(m =>
            {
                var values = itemValues.SelectMany(x => x.PostDataDetails.SelectMany(t => t.Values.Select(z => new { t.Time, z.Value, Id = z.Code })));
                return new
                {
                    Item = m,
                    Data = values
                };
            }).ToList();

            return data;
        }
    }
}
