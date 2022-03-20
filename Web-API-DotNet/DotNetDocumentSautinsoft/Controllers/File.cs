using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.StaticFiles;
using DotNetDocumentSautinsoft.Services;
using DotNetDocumentSautinsoft.Models;

namespace DotNetDocumentSautinsoft.Controllers
{
    [Route("api/[controller]")]

    //https://localhost:44352/api/file/upload?tExtension=.pdf
    //https://localhost:44352/api/file/download?FileName=637821225331658465.pdf
    [ApiController]
    public class FileController : ControllerBase
    {
        [Route("Upload")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload(IFormFile file, string tExtension, CancellationToken cancellationToken)
        {
            if (tExtension != "null" && file.Name != "null")
            {
                FileDTO fileDTO = new FileDTO();
                string fileName = "", WithoutExFileName = "";
                try
                {
                    var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                    fileDTO.FileActualName = Path.GetFileNameWithoutExtension(file.FileName);
                    WithoutExFileName = DateTime.Now.Ticks.ToString();
                    fileDTO.FileUniqueName = WithoutExFileName;
                    fileDTO.DestinationFileExt = tExtension;
                    FileDAL.SaveFileInDB(fileDTO);
                    fileName = WithoutExFileName + extension;
                    var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "ClientFiles\\Upload");
                    var pathDownloadBuilt = Path.Combine(Directory.GetCurrentDirectory(), "ClientFiles\\Download");
                    if (!Directory.Exists(pathBuilt))
                    {
                        Directory.CreateDirectory(pathBuilt);
                    }
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "ClientFiles\\Upload",
                       fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    if ("Success" != DocsSautin.Convert(pathBuilt.ToString(), pathDownloadBuilt.ToString(), WithoutExFileName, extension.ToString(), tExtension))
                    {
                        return BadRequest("Conversion Library Issue");
                    }
                    DocsSautin.directoryCleaner(pathBuilt);
                    DocsSautin.directoryDownloadCleaner(pathDownloadBuilt);
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
                return Ok(fileDTO.FileUniqueName);
            }
            else
            {
                return BadRequest("Did't Pass the Target Extension");
            }
        }
        [HttpGet("Download")]
        public async Task<ActionResult> Download(string FileName)
        {
            String uniqueName = FileName;
            var fileDTO = FileDAL.GetFileByUniqueID(uniqueName);
            try
            {
                if (fileDTO != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "ClientFiles\\Download",
                       FileName + fileDTO.DestinationFileExt);

                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(filePath, out var contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    return File(bytes, contentType, fileDTO.FileActualName + fileDTO.DestinationFileExt);

                }
                else
                {
                    return BadRequest("Did't Pass the File Name");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Did't Pass the File Name");

            }
        }
    }
}
