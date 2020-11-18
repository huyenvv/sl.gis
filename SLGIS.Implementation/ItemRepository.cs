using SLGIS.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace SLGIS.Implementation
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        private readonly IFileService _fileService;
        public ItemRepository(IMongoDatabase db, IFileService fileService) : base(db)
        {
            _fileService = fileService;
        }

        public Task<Item> CreateNewForlder(Guid? parentId, string createdBy, string folderName = "New Folder")
        {
            var item = new Item
            {
                Title = "New Folder",
                ParentId = parentId,
                IsFolder = true,
                UpdatedBy = createdBy,
            };

            return AddAsync(item);
        }

        public IEnumerable<Item> FindBy(Guid? parentId, string username)
        {
            if (parentId.HasValue)
            {
                return Find(m => m.ParentId == parentId && (m.UpdatedBy == username || !m.IsPrivate))
                    .OrderByDescending(m => m.IsFolder);
            }
            else
            {
                return Find(m => !m.ParentId.HasValue && (m.UpdatedBy == username || !m.IsPrivate))
                    .OrderByDescending(m => m.IsFolder);
            }
        }

        public async Task Rename(Guid id, string newName)
        {
            var item = Get(id);
            item.Title = newName;
            await UpdateAsync(item);
        }

        public async Task ToggleShare(Guid id)
        {
            var item = Get(id);
            item.IsPrivate = !item.IsPrivate;
            await UpdateAsync(item);
        }

        public async Task<IEnumerable<Item>> UploadFile(Guid? parentId, string createdBy, IEnumerable<IFormFile> files)
        {
            var result = new List<Item>();
            foreach (var file in files)
            {
                var filePath = await _fileService.UpsertAsync(await file.GetBytes(), file.FileName, null, true);
                var item = new Item
                {
                    Title = file.FileName,
                    ParentId = parentId,
                    FullPath = filePath,
                    CreatedBy = createdBy,
                    UpdatedBy = createdBy,
                    IsFolder = false,
                    IsPrivate = true,
                    Size = Math.Round((double)(file.Length / 1024)),
                };

                result.Add(await UpsertAsync(item));
            }

            return result;
        }

        public override Task DeleteAsync(Guid id)
        {
            var item = Get(id);
            if (!item.IsFolder)
            {
                _fileService.DeleteAsync(item.FullPath);
            }
            
            return base.DeleteAsync(id);
        }
    }
}
