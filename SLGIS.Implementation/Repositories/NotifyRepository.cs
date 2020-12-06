using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Core.Repositories;

namespace SLGIS.Implementation.Repositories
{
    public class NotifyRepository : BaseRepository<Notify>, INotifyRepository
    {
        public NotifyRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
