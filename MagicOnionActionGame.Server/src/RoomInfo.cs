using System.Collections.Concurrent;
using System.Linq;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample
{
    class RoomInfo
    {
        const int defaultMaxPlayers = 10;

        int MaxPlayers { get; }
        string Name { get; }

        private ConcurrentDictionary<string, Player> _playerList;

        public RoomInfo(string name)
        {
            this.Name = name;
            this.MaxPlayers = defaultMaxPlayers;
            this._playerList = new ConcurrentDictionary<string, Player>();
        }

        public RoomInfo(string name, int maxPlayers)
        {
            this.Name = name;
            this.MaxPlayers = maxPlayers;
            this._playerList = new ConcurrentDictionary<string, Player>();
        }

        public Player AddPlayer(string userId, string playerName)
        {
            Player player = new Player() { UserId = userId, Name = playerName, ActorNumber = -1 };
            player.ActorNumber = FindNewActorNumber();

            if (_playerList.Count < MaxPlayers)
            {
                _playerList.TryAdd(userId, player);
            }

            return player;
        }

        public Player GetPlayer(string userId)
        {
            Player player = null;
            _playerList.TryGetValue(userId, out player);
            return player;
        }

        public Player[] GetPlayers()
        {
            return _playerList.Values.ToArray();
        }

        public bool RemovePlayer(string userId)
        {
            Player player;
            return _playerList.TryRemove(userId, out player);
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
