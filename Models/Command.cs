using System;

namespace Terarecon.Eureka.Cardiac.NotificationService.Models
{
    public class Command<T>
    {
        public DateTimeOffset CreatedAt { get; set; }
        public string Source { get; set; }
        public T Args { get; set; }
        public string Name { get; set; }
    }
}