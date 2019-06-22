using System.Collections.Generic;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample
{
    class RoomManager
    {
        public static readonly RoomManager Instance = new RoomManager();

        private Dictionary<string, RoomInfo> _roomList;
        private Dictionary<string, string> _playerRoomMap;

        private RoomManager()
        {
            _roomList = new Dictionary<string, RoomInfo>();
            _playerRoomMap = new Dictionary<string, string>();
        }

        public RoomInfo CreateRoom(string roomName)
        {
            RoomInfo roomInfo = new RoomInfo(roomName);
            _roomList.Add(roomName, roomInfo);
            return roomInfo;
        }

        public RoomInfo CreateRoom(string roomName, int maxPlayers)
        {
            RoomInfo roomInfo = new RoomInfo(roomName, maxPlayers);
            _roomList.Add(roomName, roomInfo);
            return roomInfo;
        }

        public void RemoveRoom(string roomName)
        {
            _roomList.Remove(roomName);
        }

        public RoomInfo GetRoom(string roomName)
        {
            RoomInfo roomInfo;
            _roomList.TryGetValue(roomName, out roomInfo);
            return roomInfo;
        }

        public Player JoinOrCreateRoom(string roomName, string playerName, string userId)
        {
            RoomInfo roomInfo;

            _roomList.TryGetValue(roomName, out roomInfo);
            if (roomInfo == null)
            {
                roomInfo = CreateRoom(roomName);
            }

            Player player = roomInfo.AddPlayer(userId, playerName);
            if (player != null)
            {
                _playerRoomMap.Add(userId, roomName);
            }

            return player;
        }

        public void LeaveRoom(string userId)
        {
            string roomName;
            RoomInfo roomInfo;

            _playerRoomMap.TryGetValue(userId, out roomName);
            _roomList.TryGetValue(roomName, out roomInfo);

            if (roomInfo != null)
            {
                roomInfo.RemovePlayer(userId);
            }
            _playerRoomMap.Remove(userId);
        }
    }
}