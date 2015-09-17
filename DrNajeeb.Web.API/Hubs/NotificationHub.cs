using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Hubs
{
    public class NotificationHub : Hub
    {
        private static Dictionary<string, dynamic> ConnectedClients = new Dictionary<string, dynamic>();
        private static Dictionary<string, dynamic> ConnectedAdmins = new Dictionary<string, dynamic>();


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

        public void RegisterAdmin(string username)
        {
            lock (ConnectedAdmins)
            {
                if (ConnectedAdmins.ContainsKey(username))
                {
                    ConnectedAdmins[username] = Clients.Caller;
                }
                else
                {
                    ConnectedAdmins.Add(username, Clients.Caller);
                }
            }
        }


        public void SendMessageReply(string notificationMessage, string toUser)
        {
            lock (ConnectedClients)
            {
                if (ConnectedClients.ContainsKey(toUser))
                {
                    dynamic client = ConnectedClients[toUser];
                    client.addMessage(notificationMessage);
                    //Clients.User(toUser).addMessage(notificationMessage);
                }
            }
        }

        public void SendUserMessage(string notificationMessage, string fromUSer)
        {
            //Context.User.Identity.Name
            //Clients.User().
            lock (ConnectedAdmins)
            {
                foreach (var item in ConnectedAdmins)
                {
                    if (ConnectedAdmins.ContainsKey(item.Key))
                    {
                        dynamic admin = ConnectedAdmins[item.Key];
                        admin.messageFromUser(notificationMessage, fromUSer);
                    }
                }
            }
        }
    }
}