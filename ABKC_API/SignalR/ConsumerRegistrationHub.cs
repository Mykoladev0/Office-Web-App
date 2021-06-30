using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CoreApp.SignalR
{
    public class ConsumerRegistrationHub : Hub
    {
        private const string GROUPNAME = "Registration Users";
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GROUPNAME);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUPNAME);
            await base.OnDisconnectedAsync(exception);
        }
    }
}