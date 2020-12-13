using System;
using System.Threading.Tasks;

namespace SLGIS.Core.Repositories
{
    public interface INotifyRepository : IBaseRepository<Notify>
    {
        Task SetReadAsync(Guid id, Guid plantId, string userId);

        int CountUnread(Guid plantId, string userId);
    }
}
