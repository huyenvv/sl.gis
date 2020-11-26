using SLGIS.Core;
using MongoDB.Driver;

namespace SLGIS.Implementation
{
    public class PostDataRepository : BaseRepository<PostData>, IPostDataRepository
    {
        public PostDataRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
