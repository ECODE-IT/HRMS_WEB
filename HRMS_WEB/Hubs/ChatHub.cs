using HRMS_WEB.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(ChatMessage message) { 
            await Clients.Others.SendAsync("RecieveMessage", message);
        }
    }
}
