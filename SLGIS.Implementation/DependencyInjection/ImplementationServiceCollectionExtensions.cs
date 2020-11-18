using AspNetCore.Identity.Mongo;
using SLGIS.Core;
using SLGIS.Implementation;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ImplementationServiceCollectionExtensions
    {
        public static IServiceCollection AddImplementation(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                    .AddSingleton<IUserRepository, UserRepository>()
                    .AddSingleton<IComputerRepository, ComputerRepository>()
                    .AddSingleton<IItemRepository, ItemRepository>()
                    .AddSingleton<IFileService, FileService>();
        }

        public static IServiceCollection AddMongoSigleton(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddIdentityMongoDbProvider<User, Role>(
                identityOptions =>
                {
                    identityOptions.Password.RequiredLength = 1;
                    identityOptions.Password.RequireLowercase = false;
                    identityOptions.Password.RequireUppercase = false;
                    identityOptions.Password.RequireNonAlphanumeric = false;
                    identityOptions.Password.RequireDigit = false;
                },
                mongoIdentityOptions => mongoIdentityOptions.ConnectionString = connectionString
                );

            var url = new MongoUrl(connectionString);
            var database = new MongoClient(url).GetDatabase(url.DatabaseName);

            return services
                    .AddSingleton(database);
        }
    }
}
