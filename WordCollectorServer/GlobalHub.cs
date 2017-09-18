using System;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using Contracts;

namespace WordCollectorServer
{
    public class GlobalHub : Hub<IClientContract>, IServerContract
    {
        List<User> Users = new List<User>();
        Dictionary<string, Game> Games = new Dictionary<string, Game>();
        Random rnd = new Random(DateTime.Now.Millisecond);

        // Подключение нового пользователя
        public void Connect(string userName)
        {
            string id = this.Context.ConnectionId;

            User user = 
                this.Users.Find(u => string.Equals(
                        u.Name, userName, 
                        StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                user = new User{ Name = userName, ConnectionId = id };
                this.Users.Add(user);

                // Посылаем сообщение текущему пользователю
                //this.Clients.Caller.OnConnected(id, userName);
            }
            else
            {
                if (user.ConnectionId != id)
                    this.Clients.Caller.OnShowMessage(
                        "Пользователь с именем [" + userName
                        + "] уже подключился, выберите другое");
                /*else
                    // Посылаем сообщение текущему пользователю
                    this.Clients.Caller.OnConnected(id, userName);*/
            }
        }

        /*// Отключение пользователя
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
        }*/

        public Tuple<string, string> CreateNewGame()
        {
            User firstUser = 
                this.Users.Find(u => u.ConnectionId == this.Context.ConnectionId);
            User secondUser = null;
            while (firstUser == secondUser || secondUser == null)
            {
                int rndUserIndex = rnd.Next(this.Users.Count);
                secondUser = this.Users[rndUserIndex];
            }

            Game game = new Game(firstUser, secondUser);
            this.Games.Add(game.Id, game);

            return new Tuple<string, string>(game.Id, secondUser.Name);
        }

        public void DoStep(string word)
        {
            throw new NotImplementedException();
        }
    }
}

