using SLGIS.Core;
using MongoDB.Driver;
using SLGIS.Core.Model;
using SLGIS.Core.Repositories;

namespace SLGIS.Implementation.Repositories
{
    public class SubstationRepository : BaseRepository<Substation>, ISubstationRepository
    {
        public SubstationRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
