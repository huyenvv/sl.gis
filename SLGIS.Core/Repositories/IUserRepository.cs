using System.Collections.Generic;
using System.Threading.Tasks;

namespace SLGIS.Core
{
    public interface IUserRepository : IBaseRepository<User>
    {
        List<User> Get();

        Task<User> GetById(string id);
        Task<List<User>> FindAsync(BaseFilter filter);
    }
}
