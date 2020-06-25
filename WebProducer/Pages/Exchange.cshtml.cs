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
    public class ExchangeModel : PageModel
    {
        [BindProperty] public string Exchange { get; set; }
        [BindProperty] public string RoutingKey { get; set; }
        [BindProperty] public string Message { get; set; }


        private readonly ILogger<ExchangeModel> logger;
        private readonly IEventBusDispatcher eventBusDispatcher;

        public ExchangeModel(ILogger<ExchangeModel> logger, IEventBusDispatcher eventBusDispatcher)
        {
            this.logger = logger;
            this.eventBusDispatcher = eventBusDispatcher;
        }

        public void OnGet()
        {

        }
        public async Task OnPost()
        {
            await eventBusDispatcher.ExchangePublishAsync(Message, Exchange, RoutingKey);
        }
    }
}
