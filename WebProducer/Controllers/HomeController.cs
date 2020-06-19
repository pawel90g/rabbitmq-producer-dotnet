using System.Threading.Tasks;
using EventsDispatcher.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebProducer.ViewModels;

namespace WebProducer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventBusDispatcher eventBusDispatcher;

        public HomeController(IEventBusDispatcher eventBusDispatcher)
        {
            this.eventBusDispatcher = eventBusDispatcher;
        }

        [HttpGet]
        public IActionResult Index() => View(new PublishMessageViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(PublishMessageViewModel request)
        {
            if (!ModelState.IsValid)
                return View("Index", request);

            await eventBusDispatcher.PublishAsync(request.Message, request.Queue);
            return View("Index", request);
        }
    }
}