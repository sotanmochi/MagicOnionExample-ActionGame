using System.Threading.Tasks;
using MagicOnion;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample.ActionGame.ServerShared.Hubs
{
    public interface IGameHub : IStreamingHub<IGameHub, IGameHubReceiver>
    {
        Task<JoinResult> JoinAsync(string roomName, string playerName, string userId);
        Task LeaveAsync();
    }

    public interface IGameHubReceiver
    {
        void OnJoin(Player player);
        void OnLeave(Player player);
    }
}
