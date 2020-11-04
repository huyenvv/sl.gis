using SLGIS.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SLGIS.Implementation
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IMongoDatabase db) : base(db)
        {
        }

        public Task<List<User>> FindAsync(BaseFilter filter)
        {
            var builder = Builders<User>.Filter;
            var filterBuilder = builder.Regex(m => m.Name, new BsonRegularExpression($"{filter.query}", "i")) |
                builder.Regex(m => m.Email, new BsonRegularExpression($"{filter.query}", "i"));

            return _collection.Find(filterBuilder).Limit(filter.limit).ToListAsync();
        }

        public List<User> Get()
        {
            return _collection.AsQueryable().ToList();
        }

        public Task<User> GetById(string id)
        {
            return _collection.Find(Builders<User>.Filter.Eq(m => m.Id, id)).FirstOrDefaultAsync();
        }

        public override async Task<User> UpdateAsync(User model)
        {
            var filter = Builders<User>.Filter.Where(x => x.Id == model.Id);
            var options = new FindOneAndReplaceOptions<User, User>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            var updatedEntity = await _collection.FindOneAndReplaceAsync(filter, model, options);
            return model;
        }
    }
}
