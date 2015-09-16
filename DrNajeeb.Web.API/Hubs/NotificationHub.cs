using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Hubs
{
    public class NotificationHub:Hub
    {
        private static Dictionary<string, dynamic> ConnectedClients = new Dictionary<string, dynamic>();

        public void RegisterClient(string username)
        {
            lock (ConnectedClients)
            {
                if (ConnectedClients.ContainsKey(username))
                {
                    ConnectedClients[username] = Clients.Caller;
                }
                else
                {
                    ConnectedClients.Add(username, Clients.Caller);
                }
            }
        }


        public void AddNotification(string notificationMessage, string toUser)
        {
            lock (ConnectedClients)
            {
                if (ConnectedClients.ContainsKey(toUser))
                {
                    dynamic client = ConnectedClients[toUser];
                    client.addMessage(notificationMessage);
                }
            }
        }
    }
}