using Grpc.Core;
using MagicOnion.Client;
using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using System.Threading.Tasks;

namespace MagicOnionExample.ActionGame.Client
{
    public class ChatHubClient : IHubClient
    {
        IChatHub _streamingHub;
        IChatHubReceiver _receiver;

        public ChatHubClient(IChatHubReceiver receiver)
        {
            this._receiver = receiver;
        }

        public async void SendMessageAsync(ChatMessage message)
        {
            await _streamingHub.SendMessageAsync(message);
        }

        void IHubClient.ConnectHub(Channel channel)
        {
            _streamingHub = StreamingHubClient.Connect<IChatHub, IChatHubReceiver>(channel, _receiver);
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
    }
}
