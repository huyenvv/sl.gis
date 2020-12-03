using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SLGIS.Core
{
    public interface IFileFolderItemRepository : IBaseRepository<FileFolderItem>
    {
        IEnumerable<FileFolderItem> FindBy(Guid? parentId, string username);
        Task ToggleShare(Guid id);
        Task Rename(Guid id, string newName);
        Task<FileFolderItem> CreateNewForlder(Guid? parentId, string createdBy, string folderName = "New Folder");
        Task<IEnumerable<FileFolderItem>> UploadFile(Guid? parentId, string createdBy, IEnumerable<IFormFile> files);
    }
}
