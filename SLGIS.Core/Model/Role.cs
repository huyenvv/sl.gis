using AspNetCore.Identity.Mongo.Model;

namespace SLGIS.Core
{
    public class Role : MongoRole
    {
        public Role() : base()
        {
        }

        public Role(string name) : base(name)
        {
        }
    }
}
