using MessagePack;

namespace MagicOnionExample.ActionGame.ServerShared.MessagePackObjects
{
    /// <summary>
    /// Room participation information
    /// </summary>
    [MessagePackObject]
    public class Player
    {
        [Key(0)]
        public string UserId { get; set; }
        [Key(1)]
        public int ActorNumber { get; set; }
        [Key(2)]
        public string Name { get; set; }
    }
}
