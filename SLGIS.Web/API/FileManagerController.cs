using Microsoft.AspNetCore.Mvc;
using SLGIS.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace SLGIS.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;
        public FileManagerController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpGet("{id?}")]
        public IEnumerable<Item> Index(Guid? id = null)
        {
            return _itemRepository.FindBy(id, User.Identity.Name);
        }

        [HttpDelete("{id:Guid}")]
        public IActionResult Delete(Guid id)
        {
            _itemRepository.DeleteAsync(id);
            return Ok();
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
