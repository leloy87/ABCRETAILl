using ABCRETAIL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRETAIL.Controllers
{
    public class BlobController : Controller
    {
        private readonly BlobStorageService _blobStorageService;

        public BlobController(BlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBlob(IFormFile file, string blobName)
        {
            if (file == null || file.Length == 0 || string.IsNullOrEmpty(blobName))
            {
                ModelState.AddModelError("", "Invalid file or blob name.");
                return View("Index");
            }

            using (var stream = file.OpenReadStream())
            {
                // Use the container name defined in the BlobStorageService
                await _blobStorageService.UploadBlobAsync(blobName, stream);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadBlob(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                ModelState.AddModelError("", "Invalid blob name.");
                return View("Index");
            }

            var blobStream = await _blobStorageService.DownloadBlobAsync(blobName);
            return File(blobStream, "application/octet-stream", blobName);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlob(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                ModelState.AddModelError("", "Invalid blob name.");
                return View("Index");
            }

            await _blobStorageService.DeleteBlobAsync(blobName);
            return RedirectToAction("Index");
        }

        private bool IsValidContainerName(string containerName)
        {
            return !string.IsNullOrWhiteSpace(containerName) &&
                   containerName.Length >= 3 &&
                   containerName.Length <= 63 &&
                   !containerName.StartsWith("-") &&
                   !containerName.EndsWith("-") &&
                   !containerName.Contains("--") &&
                   containerName.All(c => char.IsLower(c) || char.IsDigit(c) || c == '-');
        }
    }
}

