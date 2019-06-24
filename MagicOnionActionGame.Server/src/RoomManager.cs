using System.Collections.Concurrent;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample
{
    class RoomManager
    {
        public static readonly RoomManager Instance = new RoomManager();

        private ConcurrentDictionary<string, RoomInfo> _roomList;
        private ConcurrentDictionary<string, string> _playerRoomMap;

        private RoomManager()
        {
            _roomList = new ConcurrentDictionary<string, RoomInfo>();
            _playerRoomMap = new ConcurrentDictionary<string, string>();
        }

        public bool CreateRoom(string roomName)
        {
            return _roomList.TryAdd(roomName, new RoomInfo(roomName));
        }

        public bool CreateRoom(string roomName, int maxPlayers)
        {
            return _roomList.TryAdd(roomName, new RoomInfo(roomName, maxPlayers));
        }

        public void RemoveRoom(string roomName)
        {
            RoomInfo roomInfo;
            _roomList.TryRemove(roomName, out roomInfo);
        }

        public RoomInfo GetRoom(string roomName)
        {
            RoomInfo roomInfo;
            _roomList.TryGetValue(roomName, out roomInfo);
            return roomInfo;
        }

        public Player JoinOrCreateRoom(string roomName, string playerName, string userId)
        {
            RoomInfo roomInfo = _roomList.GetOrAdd(roomName, new RoomInfo(roomName));

            Player player = roomInfo.GetPlayer(userId);
            if (player == null)
            {
                player = roomInfo.AddPlayer(userId, playerName);
                if (player.ActorNumber >= 0)
                {
                    _playerRoomMap.TryAdd(userId, roomName);
                }
            }

            return player;
        }

        public bool LeaveRoom(string userId)
        {
            string roomName;
            RoomInfo roomInfo;

            if (_playerRoomMap.TryRemove(userId, out roomName))
            {
                _roomList.TryGetValue(roomName, out roomInfo);

                if (roomInfo != null)
                {
                    return roomInfo.RemovePlayer(userId);
                }
            }

            return false;
        }
    }
}