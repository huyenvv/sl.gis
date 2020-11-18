using System.IO;
using System.Threading.Tasks;

namespace SLGIS.Core
{
    public interface IFileService
    {
        string GetFullPath(string fileName);

        Task<Stream> GetAsync(string fileName);

        Task<string> UpsertAsync(byte[] bytes, string fileName, string rootFolder, bool createSubDateFolder = true);

        Task<string> UpsertAsync(string base64String, string fileName, string rootFolder, bool createSubDateFolder = true);

        Task DeleteAsync(string path);
    }
}
