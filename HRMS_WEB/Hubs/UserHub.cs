using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Hubs
{
    public class UserHub : Hub
    {
        public async Task SendUsercount(int usercount)
        {
            await Clients.Others.SendAsync("RecieveUsercount", usercount);
        }
    }
}
