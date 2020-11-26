using SLGIS.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SLGIS.Implementation
{
    public class FileService: IFileService
    {
        const string _rootFolder = "wwwroot/uploads/";
        public string GetFullPath(string fileName)
        {
            return "/";
        }

        public Task<string> UpsertAsync(byte[] bytes, string fileName, string rootFolder, bool createSubDateFolder = true)
        {
            var subFolder = createSubDateFolder ? $"{DateTime.Now:yyyy/MM}/" : null;
            rootFolder = string.IsNullOrEmpty(rootFolder) ? _rootFolder : $"{rootFolder}/";

            var folderDestination = $"{rootFolder}{subFolder}";
            if (!Directory.Exists(folderDestination))
            {
                Directory.CreateDirectory(folderDestination);
            }

            fileName = $"{rootFolder}{subFolder}{fileName}";

            using (FileStream output = new FileStream(fileName, FileMode.Create))
            {
                using var input = new MemoryStream(bytes);
                input.CopyTo(output);
            }

            return Task.FromResult<string>(fileName);
        }

        public Task<string> UpsertAsync(string base64String, string fileName, string rootFolder, bool createSubDateFolder = true)
        {
            byte[] data = Convert.FromBase64String(base64String);
            return UpsertAsync(data, fileName, rootFolder, createSubDateFolder);
        }

        public Task DeleteAsync(string path)
        {
            if (string.IsNullOrEmpty(path)) return Task.CompletedTask;
            if (File.Exists(path))
                File.Delete(path);
            return Task.CompletedTask;
        }

        public async Task<Stream> GetAsync(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return memory;
        }
    }
}
