using MessagePack;

namespace MagicOnionExample.ActionGame.ServerShared.MessagePackObjects
{
    /// <summary>
    /// Room participation information
    /// </summary>
    [MessagePackObject]
    public class JoinResult
    {
        [Key(0)]
        public Player LocalPlayer { get; set; }
    }
}
