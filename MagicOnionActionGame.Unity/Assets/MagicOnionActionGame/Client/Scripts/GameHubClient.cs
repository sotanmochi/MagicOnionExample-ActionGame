using Grpc.Core;
using MagicOnion.Client;
using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using System;
using System.Threading.Tasks;

namespace MagicOnionExample.ActionGame.Client
{
    public class GameHubClient : IHubClient
    {
        IGameHub _streamingHub;
        IGameHubReceiver _receiver;

        public Action AfterJoinHub;
        public Action BeforeLeaveHub;

        public GameHubClient(IGameHubReceiver receiver)
        {
            this._receiver = receiver;
        }

        void IHubClient.ConnectHub(Channel channel)
        {
            _streamingHub = StreamingHubClient.Connect<IGameHub, IGameHubReceiver>(channel, _receiver);
        }

        async Task IHubClient.DisconnectHubAsync()
        {
            await _streamingHub.DisposeAsync();
        }

        Task<JoinResult> IHubClient.JoinHubAsync(string roomName, string playerName, string userId)
        {
            return _streamingHub.JoinAsync(roomName, playerName, userId);
        }

        async void IHubClient.LeaveHubAsync()
        {
            await _streamingHub.LeaveAsync();
        }

        void IHubClient.AfterJoinHub()
        {
            this.AfterJoinHub?.Invoke();
        }

        void IHubClient.BeforeLeaveHub()
        {
            this.BeforeLeaveHub?.Invoke();
        }
    }
}
