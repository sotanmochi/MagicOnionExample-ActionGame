using System.Threading.Tasks;
using MagicOnion;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample.ActionGame.ServerShared.Hubs
{
    public interface IChatHub : IStreamingHub<IChatHub, IChatHubReceiver>
    {
        Task SendMessageAsync(ChatMessage message);
        Task<JoinResult> JoinAsync(string roomName, string playerName, string userId);
        Task LeaveAsync();
    }

    public interface IChatHubReceiver
    {
        void OnReceivedMessage(ChatMessage message);
        void OnJoin(Player player);
        void OnLeave(Player player);
    }
}
