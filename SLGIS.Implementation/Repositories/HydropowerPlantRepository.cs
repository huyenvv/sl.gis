using SLGIS.Core;
using MongoDB.Driver;

namespace SLGIS.Implementation
{
    public class HydropowerPlantRepository : BaseRepository<HydropowerPlant>, IHydropowerPlantRepository
    {
        public HydropowerPlantRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
