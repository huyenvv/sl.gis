using SLGIS.Core;
using MongoDB.Driver;

namespace SLGIS.Implementation
{
    public class FactoryRepository : BaseRepository<Factory>, IFactoryRepository
    {
        public FactoryRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
