using MongoDB.Driver;
using SLGIS.Core;
using SLGIS.Core.Repositories;

namespace SLGIS.Implementation.Repositories
{
    public class ReportRepository : BaseRepository<Report>, IReportRepository
    {
        public ReportRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
