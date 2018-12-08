
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalR.Server
{
    public class SignalRHub : Hub
    {
        public async Task Attach(string id) =>
            await Groups.AddToGroupAsync(Context.ConnectionId, id);

        public async Task Detach(string id) =>
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, id);
    }
}