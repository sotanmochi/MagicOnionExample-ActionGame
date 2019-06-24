using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using UnityEngine;

namespace MagicOnionExample.ActionGame.Client
{
    public class ChatHubComponent : MonoBehaviour, IChatHubReceiver
    {
        public static ChatHubComponent Instance { get { return _instance; } }

        public FixedSizeQueue<ChatMessage> MessageList { get { return _messageList; } }
        private FixedSizeQueue<ChatMessage> _messageList;

        public delegate void OnReceivedMessageHandler(ChatMessage message);
        public OnReceivedMessageHandler OnReceivedChatMessage;

        private static ChatHubComponent _instance;
        private ChatHubClient _chatHubClient;

        void Awake()
        {
            _instance = this;

            _chatHubClient = new ChatHubClient(this);
            _chatHubClient.AfterJoinHub += AfterJoinChatHub;
            _chatHubClient.BeforeLeaveHub += BeforeLeaveChatHub;

            MagicOnionNetwork.RegisterHubClient(_chatHubClient);

            _messageList = new FixedSizeQueue<ChatMessage>(20);
        }

        public void SendMessageAsync(string msgtext)
        {
            if (MagicOnionNetwork.IsJoined)
            {
                ChatMessage message = new ChatMessage();
                message.ActorNumber = MagicOnionNetwork.LocalPlayer.ActorNumber;
                message.PlayerName = MagicOnionNetwork.LocalPlayer.Name;
                message.MessageText = msgtext;
                _chatHubClient.SendMessageAsync(message);
            }
        }

        void AfterJoinChatHub()
        {
            SendMessageAsync("***** Join room *****");
        }

        void BeforeLeaveChatHub()
        {
            SendMessageAsync("***** Leave room *****");
        }

        void IChatHubReceiver.OnReceivedMessage(ChatMessage message)
        {
            Debug.Log("OnReceivedMessage: " + message.MessageText);
            _messageList.Enqueue(message);
            OnReceivedChatMessage?.Invoke(message);
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
