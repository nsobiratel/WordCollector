using System;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordCollectorServer
{
    public class GlobalHub : Hub
    {
        List<User> Users = new List<User>();

        // Подключение нового пользователя
        public void Connect(string userName)
        {
            string id = this.Context.ConnectionId;

            User user = this.Users.Find(u => u.Name);

            if (user == null)
            {
                user = new User{ Name = userName, ConnectionId = id };
                this.Users.Add(user);

                // Посылаем сообщение текущему пользователю
                this.Clients.Caller.OnConnected(id, userName);
            }
            else
            {
                if (user.ConnectionId != id)
                    this.Clients.Caller.AddMessage(
                        userName, "Пользователь с таким именем уже подключился, выберите другое");
                else
                    // Посылаем сообщение текущему пользователю
                    this.Clients.Caller.OnConnected(id, userName);
            }
        }

        // Отключение пользователя
        public override Task OnDisconnected(bool stopCalled)
        {
            User user = 
                this.Users.FirstOrDefault(
                    x => x.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                this.Users.Remove(user);
                Clients.All.onUserDisconnected(
                    user.ConnectionId, user.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}

