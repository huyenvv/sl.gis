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
    //[Authorize(Roles = Constant.Role.SupperAdmin + "," + Constant.Role.Admin)]
    public class HydropowerController : ControllerBase
    {
        private readonly IHydropowerPlantRepository _hydropowerPlantRepository;
        public HydropowerController(IHydropowerPlantRepository hydropowerPlantRepository)
        {
            _hydropowerPlantRepository = hydropowerPlantRepository;
        }

        [HttpGet("{id?}")]
        public IEnumerable<FileFolderItem> Index(Guid? id = null)
        {
            return _hydropowerPlantRepository.FindBy(id, User.Identity.Name);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _hydropowerPlantRepository.DeleteAsync(id);
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
            var item = await _hydropowerPlantRepository.GetAsync(id);
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
            _hydropowerPlantRepository.ToggleShare(id);
            return Ok();
        }

        [HttpPost("{id:Guid}/rename")]
        public IActionResult Rename(Guid id, [FromBody][Required] string name)
        {
            _hydropowerPlantRepository.Rename(id, name);
            return Ok();
        }

        [HttpPost("createFolder")]
        public async Task<IActionResult> NewFolder([FromBody] Guid? parentId)
        {
            var result = await _hydropowerPlantRepository.CreateNewForlder(parentId, User.Identity.Name);
            return Ok(result);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadModel model)
        {
            var result = await _hydropowerPlantRepository.UploadFile(model.ParentId, User.Identity.Name, model.Files);
            return Ok(result);
        }
    }
}
