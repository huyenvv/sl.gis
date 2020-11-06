using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(SLGIS.Web.Areas.Identity.IdentityHostingStartup))]
namespace SLGIS.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                services.AddMongoSigleton(context.Configuration);
            });
        }
    }
}