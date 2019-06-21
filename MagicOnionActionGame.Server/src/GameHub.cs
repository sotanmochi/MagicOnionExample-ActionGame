using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
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

        public async Task<JoinResult> JoinAsync(string roomName, string playerName, string userId)
        {
            Guid connectionId = this.Context.ContextId;
            GrpcEnvironment.Logger.Debug("GameHub - ConnectionId: " + connectionId);

            if (storage != null)
            {
                Player player = storage.Get(connectionId);
                if (player != null)
                {
                    return new JoinResult() { LocalPlayer = player, RoomPlayers = storage.AllValues.ToArray() }; // Already joined
                }
            }

            bool success = false;
            self = new Player() { ActorNumber = -1, Name = playerName, UserId = userId };
            (success, room, storage) = await Group.TryAddAsync(roomName, MAX_PLAYERS, true, self);

            if (!success)
            {
                return new JoinResult() { LocalPlayer = self, RoomPlayers = (Player[])Enumerable.Empty<Player>() }; // Could not join
            }

            self.ActorNumber = FindNewActorNumber(); // Update player.ActorNumber
            // BroadcastExceptSelf(room).OnJoin(self);
            Broadcast(room).OnJoin(self);

            return new JoinResult() { LocalPlayer = self, RoomPlayers = storage.AllValues.ToArray() };
        }

        private int FindNewActorNumber()
        {
            var playerList = storage.AllValues.ToDictionary(player => player.ActorNumber);

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
