using ABCRETAIL.Models;
using ABCRETAIL.Services;
using Microsoft.AspNetCore.Mvc;
using ABCRETAIL;

namespace ABCRETAIL.Controllers
{
    public class FileController : Controller
    {
       
            private readonly FileService _fileService;

            public FileController(FileService fileService)
            {
                _fileService = fileService;
            }

            // Display the file upload form
            public IActionResult Index()
            {
                return View(new FileViewModel());
            }

            // Handle file uploads
            [HttpPost]
            public async Task<IActionResult> Upload(FileViewModel model)
            {
                if (model.File == null || model.File.Length == 0)
                {
                    ModelState.AddModelError("", "No file selected for upload.");
                    return View("Index", model);
                }

                // Save the file temporarily
                string tempFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                // Upload the file to Azure Files
                await _fileService.UploadFileAsync(tempFilePath, model.File.FileName);

                // Clean up the temporary file
                System.IO.File.Delete(tempFilePath);

                return RedirectToAction("Index");
            }

            // Handle file downloads
            [HttpGet]
            public async Task<IActionResult> Download(string fileName)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return BadRequest("File name is required.");
                }

                string tempFilePath = Path.GetTempFileName();
                await _fileService.DownloadFileAsync(fileName, tempFilePath);

                var fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);
                // Clean up the temporary file
                System.IO.File.Delete(tempFilePath);

                return File(fileBytes, "application/octet-stream", fileName);
            }
        }
    }

