using Microsoft.AspNetCore.SignalR;

namespace RESTfulAPI_Technical_Task_.Composable
{
    public class SignaRHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
