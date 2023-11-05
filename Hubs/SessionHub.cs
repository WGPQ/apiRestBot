using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Hubs
{
    public class SessionHub:Hub
    {
        public Task OpenSession(string user)
        {
            return Clients.All.SendAsync("ReciveOne",user,"Hola");
        }
    }
}
