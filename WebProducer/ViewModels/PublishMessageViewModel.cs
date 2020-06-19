using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebProducer.ViewModels
{
    public class PublishMessageViewModel
    {
        [Required]
        public string Message { get; set; }
        [Required]
        public string Queue { get; set; }
        // [Required]
        public string RoutingKey { get; set; }
    }
}