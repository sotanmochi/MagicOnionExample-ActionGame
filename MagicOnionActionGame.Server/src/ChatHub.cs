using System;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Server.Hubs;
using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample.ActionGame.Server
{
    class ChatHub : StreamingHubBase<IChatHub, IChatHubReceiver>, IChatHub
    {
        IGroup group;
        Player self;

        public async Task SendMessageAsync(ChatMessage message)
        {
            GrpcEnvironment.Logger.Debug("SendMessageAsync @ChatHub");
            Broadcast(group).OnReceivedMessage(message);
            await Task.CompletedTask;
        }

        public async Task SendMessageExceptSelfAsync(ChatMessage message)
        {
            GrpcEnvironment.Logger.Debug("SendMessageExeptSelfAsync @ChatHub");
            BroadcastExceptSelf(group).OnReceivedMessage(message);
            await Task.CompletedTask;
        }

        public async Task<JoinResult> JoinAsync(string roomName, string playerName, string userId)
        {
            Guid connectionId = this.Context.ContextId;
            GrpcEnvironment.Logger.Debug("ChatHub - ConnectionId: " + connectionId);

            self = RoomManager.Instance.JoinOrCreateRoom(roomName, playerName, userId);
            Player[] roomPlayers = RoomManager.Instance.GetRoom(roomName).GetPlayers();

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
            GrpcEnvironment.Logger.Debug("LeavAsync @ChatHub");

            if (RoomManager.Instance.LeaveRoom(self.UserId))
            {
                await group.RemoveAsync(this.Context);
                Broadcast(group).OnLeave(self);
            }
        }
    }
}
