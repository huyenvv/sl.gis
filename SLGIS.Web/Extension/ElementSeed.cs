using Microsoft.Extensions.DependencyInjection;
using SLGIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Web
{
    public static class ElementSeed
    {
        public static async Task InitElement(this IServiceProvider serviceProvider)
        {
            var elementRepository = serviceProvider.GetRequiredService<IElementRepository>();
            if (elementRepository.Find(m => true).Any())
            {
                return;
            }

            var elements = new List<Element>()
            {
                new Element { Title = "Lưu lượng qua Tuabin", Code = "LuuLuongQuaTuaBin", Unit = "m3/s" },
                new Element { Title = "Lưu lượng đến hồ", Code = "LuuLuongDenHo", Unit = "m3/s" },
                new Element { Title = "Lưu lượng xả sau đập", Code = "LuuLuongXaSauDap", Unit = "m3/s" },
                new Element { Title = "Mực nước hạ lưu", Code = "MucNuocHaLuu", Unit = "m" },
                new Element { Title = "Mực nước hồ", Code = "MucNuocHoChua", Unit = "m" },
            };

            await elementRepository.AddRangeAsync(elements);
        }
    }
}
