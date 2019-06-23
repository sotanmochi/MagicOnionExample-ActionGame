using MessagePack;

namespace MagicOnionExample.ActionGame.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class ChatMessage
    {
        [Key(0)]
        public int ActorNumber { get; set; }
        [Key(1)]
        public string PlayerName { get; set; }
        [Key(2)]
        public string MessageText { get; set; }
    }
}
