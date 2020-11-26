using SLGIS.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace SLGIS.Implementation
{
    public class FileFolderItemRepository : BaseRepository<FileFolderItem>, IFileFolderItemRepository
    {
        private readonly IFileService _fileService;
        public FileFolderItemRepository(IMongoDatabase db, IFileService fileService) : base(db)
        {
            _fileService = fileService;
        }

        public Task<FileFolderItem> CreateNewForlder(Guid? parentId, string createdBy, string folderName = "New Folder")
        {
            var item = new FileFolderItem
            {
                Title = "New Folder",
                ParentId = parentId,
                IsFolder = true,
                UpdatedBy = createdBy,
                IsPrivate = false,
            };

            return AddAsync(item);
        }

        public IEnumerable<FileFolderItem> FindBy(Guid? parentId, string username)
        {
            if (parentId.HasValue)
            {
                return Find(m => m.ParentId == parentId && (m.UpdatedBy == username || m.IsFolder || !m.IsPrivate))
                    .OrderByDescending(m => m.IsFolder);
            }
            else
            {
                return Find(m => !m.ParentId.HasValue && (m.UpdatedBy == username || m.IsFolder || !m.IsPrivate))
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

        public async Task<IEnumerable<FileFolderItem>> UploadFile(Guid? parentId, string createdBy, IEnumerable<IFormFile> files)
        {
            var result = new List<FileFolderItem>();
            foreach (var file in files)
            {
                var filePath = await _fileService.UpsertAsync(await file.GetBytes(), file.FileName, null, true);
                var item = new FileFolderItem
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

        public override async Task DeleteAsync(Guid id)
        {
            var item = Get(id);
            if (!item.IsFolder)
            {
                _ = _fileService.DeleteAsync(item.FullPath);
            }

            if (item.IsFolder && Find(m => m.ParentId == id).Any())
            {
                throw new InvalidOperationException("Can not delete folder that has some folders or files");
            }

            await base.DeleteAsync(id);
        }
    }
}
