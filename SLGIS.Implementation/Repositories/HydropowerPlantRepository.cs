using SLGIS.Core;
using MongoDB.Driver;
using System.Threading.Tasks;
using System;

namespace SLGIS.Implementation
{
    public class HydropowerPlantRepository : BaseRepository<HydropowerPlant>, IHydropowerPlantRepository
    {
        public HydropowerPlantRepository(IMongoDatabase db) : base(db)
        {
        }
        public Task SetLocationAsync(Guid id, Location location)
        {
            var filter_id = Builders<HydropowerPlant>.Filter.Eq("_id", id);
            return _collection.UpdateOneAsync(filter_id, Builders<HydropowerPlant>.Update.Set(m => m.Location, location));
        }
    }
}
