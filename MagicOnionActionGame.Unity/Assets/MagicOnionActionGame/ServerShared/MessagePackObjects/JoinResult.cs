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
        public int LocalPlayerId { get; set; }
        [Key(1)]
        public Player[] RoomPlayers { get; set; }
    }
}
