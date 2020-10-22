using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatSample.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send()
        {
            for (int i = 0; i < 10; i++)
            {
                await Clients.All.SendAsync("broadcastMessage", $"'Patient {i}' results are ready to view");
                await Task.Delay(300);
            }
            // Call the broadcastMessage method to update clients.
        }
    }
}