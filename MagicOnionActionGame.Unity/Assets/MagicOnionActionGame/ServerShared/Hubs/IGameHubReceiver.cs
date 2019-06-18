using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;

namespace MagicOnionExample.ActionGame.ServerShared.Hubs
{
    public interface IGameHubReceiver
    {
        void OnJoin(Player player);
        void OnLeave(Player player);
    }
}
