using System;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Server.Hubs;
using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample.ActionGame.Server
{
    class GameHub : StreamingHubBase<IGameHub, IGameHubReceiver>, IGameHub
    {
        IGroup group;
        Player self;

        public async Task<JoinResult> JoinAsync(string roomName, string playerName, string userId)
        {
            Guid connectionId = this.Context.ContextId;
            GrpcEnvironment.Logger.Debug("GameHub - ConnectionId: " + connectionId);

            self = RoomManager.Instance.JoinOrCreateRoom(roomName, playerName, userId);
            Player[] roomPlayers = RoomManager.Instance.GetRoom(roomName).GetPlayers();

            if (self.ActorNumber >= 0)
            {
                this.group = await Group.AddAsync(roomName);
            }

            // BroadcastExceptSelf(room).OnJoin(self);
            Broadcast(group).OnJoin(self);

            return new JoinResult() { LocalPlayer = self, RoomPlayers = roomPlayers };
        }

        public async Task LeaveAsync()
        {
            GrpcEnvironment.Logger.Debug("LeavAsync @GameHub");

            RoomManager.Instance.LeaveRoom(self.UserId);

            Broadcast(group).OnLeave(self);
            await group.RemoveAsync(this.Context);
        }
    }
}
