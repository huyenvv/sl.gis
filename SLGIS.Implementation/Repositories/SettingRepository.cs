using SLGIS.Core;
using MongoDB.Driver;

namespace SLGIS.Implementation
{
    public class SettingRepository : BaseRepository<Setting>, ISettingRepository
    {
        public SettingRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
