using SLGIS.Core.Model;
using System;
using System.Threading.Tasks;

namespace SLGIS.Core
{
    public interface IHydropowerPlantRepository : IBaseRepository<HydropowerPlant>
    {
        Task SetLocationAsync(Guid id, Location location);
    }
}
