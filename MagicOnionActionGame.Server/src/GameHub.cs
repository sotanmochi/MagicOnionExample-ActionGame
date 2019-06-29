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
        string currentRoom;

        public async Task MoveAsync(PlayerCharacterParameter param)
        {
            BroadcastExceptSelf(group).OnMove(param);
        }

        public async Task<JoinResult> JoinAsync(string roomName, string playerName, string userId)
        {
            Guid connectionId = this.Context.ContextId;
            GrpcEnvironment.Logger.Debug("GameHub - ConnectionId: " + connectionId);

            self = RoomManager.Instance.JoinOrCreateRoom(roomName, playerName, userId);
            Player[] roomPlayers = RoomManager.Instance.GetRoom(roomName).GetPlayers();

            GrpcEnvironment.Logger.Debug("GameHub - PlayerCount: " + roomPlayers.Length);
            currentRoom = roomName;

            if (self.ActorNumber >= 0)
            {
                this.group = await Group.AddAsync(roomName);
                BroadcastExceptSelf(group).OnJoin(self);
                // Broadcast(group).OnJoin(self);
            }

            return new JoinResult() { LocalPlayer = self };
        }

        public async Task LeaveAsync()
        {
            GrpcEnvironment.Logger.Debug("LeavAsync @GameHub");

            RoomManager.Instance.LeaveRoom(self.UserId);
            Player[] roomPlayers = RoomManager.Instance.GetRoom(currentRoom).GetPlayers();
            GrpcEnvironment.Logger.Debug("GameHub - PlayerCount: " + roomPlayers.Length);

            await group.RemoveAsync(this.Context);
            Broadcast(group).OnLeave(self);
        }
    }
}
