using SLGIS.Core;
using MongoDB.Driver;

namespace SLGIS.Implementation
{
    public class ElementRepository : BaseRepository<Element>, IElementRepository
    {
        public ElementRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
