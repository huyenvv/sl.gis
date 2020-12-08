using System;
using System.Threading.Tasks;

namespace SLGIS.Core.Repositories
{
    public interface INotifyRepository : IBaseRepository<Notify>
    {
        Task SetReadAsync(Guid id, string userId);
    }
}
