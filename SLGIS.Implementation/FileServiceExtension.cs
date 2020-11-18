using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace SLGIS.Implementation
{
    public static class FileExtension
    {
        public static async Task<byte[]> GetBytes(this IFormFile formFile)
        {
            if (formFile == null) return null;

            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
