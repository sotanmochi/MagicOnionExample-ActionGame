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
        public int Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
    }
}
