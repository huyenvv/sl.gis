using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLGIS.Implementation.Repositories
{
    public class NotifyRepository : BaseRepository<Notify>, INotifyRepository
    {
        public NotifyRepository(IMongoDatabase db) : base(db)
        {
        }

        public int CountUnread(Guid plantId, string userId)
        {
            return Find(m => (m.IsAll || m.ToPlantIds.Contains(plantId))
                            && !m.ReadPlants.Any(n => n.Key == plantId && n.Value.Any(z => z.UserId == userId.ToString()))).Count();
        }

        public async Task SetReadAsync(Guid id, Guid plantId, string userId)
        {
            var notify = await GetAsync(id);
            if (notify.ReadPlants.Any(m => m.Key == plantId))
            {
                foreach (var item in notify.ReadPlants.Where(m => m.Key == plantId))
                {
                    item.Value.Add(new ReadNotifyInfo { UserId = userId });
                }
            }
            else
            {
                notify.ReadPlants.Add(new KeyValuePair<Guid, List<ReadNotifyInfo>>(plantId, new List<ReadNotifyInfo> { new ReadNotifyInfo { UserId = userId } }));
            }

            await UpdateAsync(notify);
        }
    }
}
