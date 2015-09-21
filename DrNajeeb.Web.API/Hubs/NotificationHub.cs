using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DrNajeeb.Web.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private static Dictionary<string, dynamic> ConnectedAdmins = new Dictionary<string, dynamic>();

        public readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();


        public void RegisterClient(string username)
        {
            //foreach (var connectionId in _connections.GetConnections(username))
            //{
            //    Clients.Client(connectionId).logout();
            //}
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
            foreach (var connectionId in _connections.GetConnections(toUser))
            {
                Clients.Client(connectionId).addMessage(notificationMessage);
            }

            //lock (ConnectedClients)
            //{
            //    if (ConnectedClients.ContainsKey(toUser))
            //    {
            //        dynamic client = ConnectedClients[toUser];
            //        client.addMessage(notificationMessage);
            //        //Clients.User(toUser).addMessage(notificationMessage);
            //    }
            //}
        }


        public void SendMessageToAll(string notificationMessage)
        {
            Clients.All.addMessage(notificationMessage);
        }

        public void SendUserMessage(string notificationMessage, string fromUSer)
        {
            Clients.All.messageFromUser(notificationMessage, fromUSer);
            //Context.User.Identity.Name
            //Clients.User().
            //lock (ConnectedAdmins)
            //{
            //    foreach (var item in ConnectedAdmins)
            //    {
            //        if (ConnectedAdmins.ContainsKey(item.Key))
            //        {
            //            dynamic admin = ConnectedAdmins[item.Key];
            //            admin.messageFromUser(notificationMessage, fromUSer);
            //        }
            //    }
            //}
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            foreach (var connectionId in _connections.GetConnections(name))
            {
                Clients.Client(connectionId).logout();
            }

            _connections.Add(name, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
    }
}