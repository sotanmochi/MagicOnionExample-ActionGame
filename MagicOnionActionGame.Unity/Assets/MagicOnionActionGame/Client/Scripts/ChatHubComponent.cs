using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using UnityEngine;

namespace MagicOnionExample.ActionGame.Client
{
    public class ChatHubComponent : MonoBehaviour, IChatHubReceiver
    {
        public static ChatHubComponent Instance;

        public FixedSizeQueue<ChatMessage> MessageList { get { return _messageList; } }
        private FixedSizeQueue<ChatMessage> _messageList;

        public delegate void OnReceivedMessageHandler(ChatMessage message);
        public OnReceivedMessageHandler OnReceivedChatMessage;

        private ChatHubClient _chatHubClient;

        void Awake()
        {
            Instance = this;

            _chatHubClient = new ChatHubClient(this);
            MagicOnionNetwork.RegisterHubClient(_chatHubClient);

            _messageList = new FixedSizeQueue<ChatMessage>(20);
        }

        public void SendMessageAsync(string msgtext)
        {
            if (MagicOnionNetwork.IsConnected)
            {
                ChatMessage message = new ChatMessage();
                message.ActorNumber = MagicOnionNetwork.LocalPlayer.ActorNumber;
                message.PlayerName = MagicOnionNetwork.LocalPlayer.Name;
                message.MessageText = msgtext;
                _chatHubClient.SendMessageAsync(message);
            }
        }

        void IChatHubReceiver.OnReceivedMessage(ChatMessage message)
        {
            _messageList.Enqueue(message);
            OnReceivedChatMessage(message);
        }

        void IChatHubReceiver.OnJoin(Player player)
        {
            Debug.Log("OnJoin@Chat - Player[" + player.ActorNumber + "]: " + player.Name);
        }

        void IChatHubReceiver.OnLeave(Player player)
        {
            Debug.Log("OnLeave@Chat - Player[" + player.ActorNumber + "]:" + player.Name);
        }
    }
}
