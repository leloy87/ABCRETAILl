using Microsoft.AspNetCore.Mvc;

using Azure;
using System;
using System.Threading.Tasks;
using ABCRETAIL.Models;
using ABCRETAIL.Services;

namespace ABCRETAIL.Controllers
{
    public class CustomerProfilesController : Controller
    {
        private readonly StorageService _storageService;

        public CustomerProfilesController(StorageService storageService)
        {
            _storageService = storageService;
        }

        // GET: CustomerProfiles
        public async Task<IActionResult> Index()
        {
            var profiles = await _storageService.GetAllCustomerProfilesAsync();
            return View(profiles);
        }

        // GET: CustomerProfiles/Details/{partitionKey}/{rowKey}
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var profile = await _storageService.GetCustomerProfileAsync(partitionKey, rowKey);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        // GET: CustomerProfiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomerProfiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber")] CustomerProfile profile)
        {
            if (!ModelState.IsValid)
            {
                profile.PartitionKey = "CustomerPartition"; // Example PartitionKey
                profile.RowKey = Guid.NewGuid().ToString(); // Generate a unique RowKey

                await _storageService.AddCustomerProfileAsync(profile);
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        // GET: CustomerProfiles/Edit/{partitionKey}/{rowKey}
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            var profile = await _storageService.GetCustomerProfileAsync(partitionKey, rowKey);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        // POST: CustomerProfiles/Edit/{partitionKey}/{rowKey}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string partitionKey, string rowKey, [Bind("PartitionKey,RowKey,FirstName,LastName,Email,PhoneNumber,ETag")] CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure PartitionKey and RowKey are preserved
                    profile.PartitionKey = partitionKey;
                    profile.RowKey = rowKey;

                    // Fetch the existing entity to get the current ETag
                    var existingProfile = await _storageService.GetCustomerProfileAsync(partitionKey, rowKey);
                    if (existingProfile == null)
                    {
                        return NotFound();
                    }

                    // Use the ETag from the existing profile
                    profile.ETag = existingProfile.ETag;

                    // Check if ETag is set
                    if (profile.ETag == null || profile.ETag.Equals(ETag.All))
                    {
                        ModelState.AddModelError("", "The entity ETag is missing or invalid.");
                        return View(profile);
                    }

                    await _storageService.UpdateCustomerProfileAsync(profile);
                    return RedirectToAction(nameof(Index));
                }
                catch (RequestFailedException ex)
                {
                    if (!await CustomerProfileExists(partitionKey, rowKey))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Log the exception message
                        Console.WriteLine($"Error updating customer profile: {ex.Message}");
                        return StatusCode(500, "Internal server error");
                    }
                }
            }
            return View(profile);
        }

        // GET: CustomerProfiles/Delete/{partitionKey}/{rowKey}
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            var profile = await _storageService.GetCustomerProfileAsync(partitionKey, rowKey);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        // POST: CustomerProfiles/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey, string ETag)
        {
            try
            {
                // Validate ETag
                if (string.IsNullOrWhiteSpace(ETag))
                {
                    ModelState.AddModelError("", "ETag is missing.");
                    return RedirectToAction(nameof(Delete), new { partitionKey, rowKey });
                }

                // Perform the delete operation
                await _storageService.DeleteCustomerProfileAsync(partitionKey, rowKey, ETag);
                return RedirectToAction(nameof(Index));
            }
            catch (RequestFailedException ex)
            {
                // Log the exception message
                Console.WriteLine($"Error deleting customer profile: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<bool> CustomerProfileExists(string partitionKey, string rowKey)
        {
            var profile = await _storageService.GetCustomerProfileAsync(partitionKey, rowKey);
            return profile != null;
        }
    }
}
