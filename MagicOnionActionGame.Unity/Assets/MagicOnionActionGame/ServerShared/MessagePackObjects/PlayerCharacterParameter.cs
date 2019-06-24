using MessagePack;
using UnityEngine;

namespace MagicOnionExample.ActionGame.ServerShared.MessagePackObjects
{
    [MessagePackObject]
    public class PlayerCharacterParameter
    {
        [Key(0)]
        public int ActorNumber { get; set; }
        [Key(1)]
        public string PlayerName { get; set; }
        [Key(2)]
        public Vector3 Position { get; set; }
        [Key(3)]
        public Quaternion Rotation { get; set; }
        [Key(4)]
        public Vector3 Move { get; set; }
        [Key(5)]
        public bool Crouch { get; set; }
        [Key(6)]
        public bool Jump { get; set; }
    }
}
