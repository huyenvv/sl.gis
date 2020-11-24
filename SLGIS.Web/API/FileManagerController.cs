using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SLGIS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace SLGIS.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constant.Role.SupperAdmin + "," + Constant.Role.Admin)]
    public class FileManagerController : ControllerBase
    {
        private readonly IFileFolderItemRepository _itemRepository;
        private readonly IFileService _fileService;
        public FileManagerController(IFileFolderItemRepository itemRepository, IFileService fileService)
        {
            _itemRepository = itemRepository;
            _fileService = fileService;
        }

        [HttpGet("{id?}")]
        public IEnumerable<FileFolderItem> Index(Guid? id = null)
        {
            return _itemRepository.FindBy(id, User.Identity.Name);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _itemRepository.DeleteAsync(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:Guid}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            var item = await _itemRepository.GetAsync(id);
            if (item == null || item.IsFolder || item.IsPrivate && item.CreatedBy != User.Identity.Name)
            {
                return BadRequest();
            }

            if (!System.IO.File.Exists(item.FullPath))
                return Content("filename not present");

            var memory = await _fileService.GetAsync(item.FullPath);
            return File(memory, Common.GetContentType(item.FullPath), Path.GetFileName(item.FullPath));
        }

        [HttpPost("{id:Guid}/toggleShare")]
        public IActionResult ToggleShare(Guid id)
        {
            _itemRepository.ToggleShare(id);
            return Ok();
        }

        [HttpPost("{id:Guid}/rename")]
        public IActionResult Rename(Guid id, [FromBody][Required] string name)
        {
            _itemRepository.Rename(id, name);
            return Ok();
        }

        [HttpPost("createFolder")]
        public async Task<IActionResult> NewFolder([FromBody] Guid? parentId)
        {
            var result = await _itemRepository.CreateNewForlder(parentId, User.Identity.Name);
            return Ok(result);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadModel model)
        {
            var result = await _itemRepository.UploadFile(model.ParentId, User.Identity.Name, model.Files);
            return Ok(result);
        }
    }
}
