using ABCRETAIL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRETAIL.Controllers
{
    public class QueueController : Controller
    {
        private readonly QueueStorageService _queueStorageService;

        public QueueController(QueueStorageService queueStorageService)
        {
            _queueStorageService = queueStorageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ModelState.AddModelError("", "Message cannot be empty.");
                return View("Index");
            }

            await _queueStorageService.SendMessageAsync(message);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ReceiveMessage()
        {
            var message = await _queueStorageService.ReceiveMessageAsync();
            ViewBag.Message = message ?? "No messages in the queue.";
            return View("Index");
        }
    }
}