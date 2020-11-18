using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SLGIS.Core
{
    public interface IItemRepository : IBaseRepository<Item>
    {
        IEnumerable<Item> FindBy(Guid? parentId, string username);
        Task ToggleShare(Guid id);
        Task Rename(Guid id, string newName);
        Task<Item> CreateNewForlder(Guid? parentId, string createdBy, string folderName = "New Folder");
        Task<IEnumerable<Item>> UploadFile(Guid? parentId, string createdBy, IEnumerable<IFormFile> files);
    }
}
