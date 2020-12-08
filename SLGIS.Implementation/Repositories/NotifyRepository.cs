using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace SLGIS.Implementation.Repositories
{
    public class NotifyRepository : BaseRepository<Notify>, INotifyRepository
    {
        public NotifyRepository(IMongoDatabase db) : base(db)
        {
        }

        public Task SetReadAsync(Guid id, string userId)
        {
            var filter_id = Builders<Notify>.Filter.Eq("_id", id);
            return _collection.UpdateOneAsync(filter_id, Builders<Notify>.Update.Push(m => m.ReadUserIds, userId));
        }
    }
}
