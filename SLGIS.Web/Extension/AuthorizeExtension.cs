using SLGIS.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace SLGIS.Web
{
    public static class AuthorizeExtension
    {
        public static void ConfigAuthorizes(this IMvcBuilder builder)
        {
            builder.AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeFolder("/Computer");
                options.Conventions.AuthorizeFolder("/User");
                options.Conventions.AllowAnonymousToPage("/User/Login");
            });
        }
    }
}