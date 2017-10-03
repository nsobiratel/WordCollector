using System;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using Contracts;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices;

namespace WordCollectorServer
{
    public class GlobalHub : Hub<IClientContract>, IServerContract
    {
        static readonly List<User> Users = new List<User>();
        static readonly Dictionary<string, Game> Games = new Dictionary<string, Game>();
        static readonly Random rnd = new Random();

        // Подключение нового пользователя
        public void Connect(string userName)
        {
            string id = this.Context.ConnectionId;

            User user = 
                Users.Find(u => string.Equals(
                        u.Name, userName, 
                        StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                user = new User{ Name = userName, ConnectionId = id };
                Users.Add(user);

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

        // Отключение пользователя
        public override Task OnDisconnected(bool stopCalled)
        {
            User user = 
                Users.FirstOrDefault(
                    x => x.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                Users.Remove(user);
                Game game;
                if (Games.TryGetValue(user.CurrentGameId, out game))
                {
                    Clients.Group(user.CurrentGameId).OnGameFinished(
                        game.GetEnemyForUser(user).Name, 
                        "Техническая победа - противник отключился от сервера");
                    game.Finish();
                    Games.Remove(user.CurrentGameId);
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public Tuple<string, string, char> CreateNewGame()
        {
            try
            {
                User firstUser = Users.Find(u => u.ConnectionId == this.Context.ConnectionId);
                User secondUser = null;

                if (Users.Count > 1)
                {
                    while (firstUser == secondUser || secondUser == null)
                    {
                        int rndUserIndex = rnd.Next(Users.Count);
                        secondUser = Users[rndUserIndex];
                    }
                }
                else
                {
                    return new Tuple<string, string, char>(string.Empty, string.Empty, char.MinValue);
                }

                Game game = new Game(firstUser, secondUser);
                Games.Add(game.Id, game);

                Task.Run(async () => await this.Groups.Add(firstUser.ConnectionId, game.Id));
                Task.Run(async () => await this.Groups.Add(secondUser.ConnectionId, game.Id));

                Clients.Client(secondUser.ConnectionId).OnGameStarted(
                    game.Id, firstUser.Name, game.currentChar.Symbol);

                return new Tuple<string, string, char>(game.Id, secondUser.Name, game.currentChar.Symbol);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return null;
            }
        }

        public bool? DoStep(string gameId, char lastChar)
        {
            Game game;
            if (!Games.TryGetValue(gameId, out game))
            {
                this.Clients.Caller.OnShowMessage(
                    "Игра с ид [" + gameId + "] не найдена на сервере");
                return false;
            }

            switch (game.ValidateStep(lastChar))
            {
                case true: 
                    this.Clients.OthersInGroup(gameId).OnCanDoStep(lastChar);
                    return true;
                case false: 
                    this.Clients.Caller.OnInvalidWord();
                    return false;
                default:
                    this.Clients.Caller.OnGameFinished(
                        "Вы выиграли", 
                        "Закончились доступные буквы, слово собрано");
                    this.Clients.OthersInGroup(gameId).OnGameFinished(
                        "Вы проиграли",
                        "Закончились доступные буквы, слово собрано");
                    return null;
            }
        }
    }
}