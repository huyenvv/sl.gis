using SLGIS.Core;
using MongoDB.Driver;
using SLGIS.Core.Model;

namespace SLGIS.Implementation
{
    public class SubstationRepository : BaseRepository<Substation>, ISubstationRepository
    {
        public SubstationRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
