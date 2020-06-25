using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventsDispatcher.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebProducer.Pages
{
    public class QueueModel : PageModel
    {
        [BindProperty] public string Queue { get; set; }
        [BindProperty] public string Message { get; set; }


        private readonly ILogger<QueueModel> logger;
        private readonly IEventBusDispatcher eventBusDispatcher;

        public QueueModel(ILogger<QueueModel> logger, IEventBusDispatcher eventBusDispatcher)
        {
            this.logger = logger;
            this.eventBusDispatcher = eventBusDispatcher;
        }

        public void OnGet()
        {

        }
        public async Task OnPost()
        {
            await eventBusDispatcher.QueuePublishAsync(Message, Queue);
        }
    }
}
