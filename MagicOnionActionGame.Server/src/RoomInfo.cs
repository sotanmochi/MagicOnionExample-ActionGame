using System.Collections.Generic;
using System.Linq;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample
{
    class RoomInfo
    {
        const int defaultMaxPlayers = 10;

        int MaxPlayers { get; }
        string Name { get; }

        private Dictionary<string, Player> _playerList = new Dictionary<string, Player>();

        public RoomInfo(string name)
        {
            this.Name = name;
            this.MaxPlayers = defaultMaxPlayers;
        }

        public RoomInfo(string name, int maxPlayers)
        {
            this.Name = name;
            this.MaxPlayers = maxPlayers;
        }

        public Player AddPlayer(string userId, string playerName)
        {
            Player player = new Player() { UserId = userId, Name = playerName, ActorNumber = -1 };
            player.ActorNumber = FindNewActorNumber();

            if (_playerList.Count < MaxPlayers)
            {
                _playerList.Add(userId, player);
            }

            return player;
        }

        public void RemovePlayer(string userId)
        {
            _playerList.Remove(userId);
        }

        public Player[] GetPlayers()
        {
            return _playerList.Values.ToArray();
        }

        private int FindNewActorNumber()
        {
            var list = _playerList.Values.ToDictionary(player => player.ActorNumber);

            for (int id = 0; id < MaxPlayers; id++)
            {
                if (!list.ContainsKey(id))
                {
                    return id;
                }
            }

            return -1;
        }
    }
}
