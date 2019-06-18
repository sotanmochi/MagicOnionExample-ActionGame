using System;
using System.Linq;
using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample.ActionGame.Server
{
    class GameHub : StreamingHubBase<IGameHub, IGameHubReceiver>, IGameHub
    {
        const int MAX_PLAYERS = 10;

        IGroup room;
        Player self;
        IInMemoryStorage<Player> storage;

        public async Task<JoinResult> JoinAsync(string roomName, string playerName)
        {
            Guid connectionId = this.Context.ContextId;
            
            if (storage != null)
            {
                Player player = storage.Get(connectionId);
                if (player != null)
                {
                    return new JoinResult() { LocalPlayerId = player.Id, RoomPlayers = storage.AllValues.ToArray() }; // Already joined
                }
            }

            bool success = false;
            self = new Player() { Id = -1, Name = playerName };
            (success, room, storage) = await Group.TryAddAsync(roomName, MAX_PLAYERS, true, self);

            if (!success)
            {
                return new JoinResult() { LocalPlayerId = -1, RoomPlayers = (Player[])Enumerable.Empty<Player>() }; // Could not join
            }

            self.Id = FindNewPlayerId(); // Update player.Id
            // BroadcastExceptSelf(room).OnJoin(self);
            Broadcast(room).OnJoin(self);

            return new JoinResult() { LocalPlayerId = self.Id, RoomPlayers = storage.AllValues.ToArray() };
        }

        private int FindNewPlayerId()
        {
            var playerList = storage.AllValues.ToDictionary(player => player.Id);

            for (int id = 0; id < MAX_PLAYERS; id++)
            {
                if (!playerList.ContainsKey(id))
                {
                    return id;
                }
            }

            return -1;
        }

        public async Task LeaveAsync()
        {
            await room.RemoveAsync(this.Context);
            Broadcast(room).OnLeave(self);
        }
    }
}
