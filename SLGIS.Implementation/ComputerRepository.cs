using SLGIS.Core;
using MongoDB.Driver;

namespace SLGIS.Implementation
{
    public class ComputerRepository : BaseRepository<Computer>, IComputerRepository
    {
        public ComputerRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
