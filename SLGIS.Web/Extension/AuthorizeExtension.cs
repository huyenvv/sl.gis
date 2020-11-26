using Microsoft.Extensions.DependencyInjection;

namespace SLGIS.Web
{
    public static class AuthorizeExtension
    {
        public static void ConfigAuthorizes(this IMvcBuilder builder)
        {
            builder.AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeFolder("/Admin/Element/");
                options.Conventions.AuthorizeFolder("/Admin/Hydropower/");
                options.Conventions.AuthorizeFolder("/User");
                options.Conventions.AuthorizeFolder("/Posting");
                options.Conventions.AuthorizeFolder("/Map");
                options.Conventions.AuthorizeFolder("/FileManager");
            });
        }
    }
}