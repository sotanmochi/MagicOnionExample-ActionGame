using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace MagicOnionExample.ActionGame.Client
{
    public class AppUI : MonoBehaviour
    {
        [SerializeField] GameHubClient Client;

        [SerializeField] InputField Host;
        [SerializeField] InputField Port;
        [SerializeField] Button ConnectButton;

        [SerializeField] InputField RoomName;
        [SerializeField] InputField PlayerName;
        [SerializeField] Button JoinButton;
        [SerializeField] Button LeaveButton;

        [SerializeField] InputField MessageInput;
        [SerializeField] Button SendButton;
        
        [SerializeField] Text MessageOutput;

        void Start()
        {
            ConnectButton.onClick.AddListener(OnConnectClicked);
            JoinButton.onClick.AddListener(OnJoinClicked);
            LeaveButton.onClick.AddListener(OnLeaveClicked);

            SendButton.onClick.AddListener(OnSendClicked);
            ChatHubComponent.Instance.OnReceivedChatMessage += OnReceivedChatMessage;
        }

        void OnConnectClicked()
        {
            MagicOnionNetwork.Connect(Host.text, int.Parse(Port.text));

            Debug.Log("*** OnConnectClicked @AppUI ***");
            Debug.Log("Connected: " + MagicOnionNetwork.IsConnected);
            Debug.Log("State: " + MagicOnionNetwork.ConnectionState);
        }

        async void OnJoinClicked()
        {
            string userId = System.Guid.NewGuid().ToString();
            bool result = await MagicOnionNetwork.JoinAsync(RoomName.text, PlayerName.text, userId);

            Debug.Log("*** OnJoinClicked @AppUI ***");
            Debug.Log("Join success: " + result);
        }

        void OnLeaveClicked()
        {
            MagicOnionNetwork.LeaveAsync();
        }

        void OnSendClicked()
        {
            ChatHubComponent.Instance.SendMessageAsync(MessageInput.text);
        }

        void OnReceivedChatMessage(ChatMessage message)
        {
            StringBuilder sb = new StringBuilder();

            ChatMessage[] messageList = ChatHubComponent.Instance.MessageList.ToArray();
            foreach (ChatMessage chatmsg in messageList)
            {
                string msg = chatmsg.PlayerName + ":\n" + "  " + chatmsg.MessageText + "\n";
                sb.Insert(0, msg);
            }

            MessageOutput.text = sb.ToString();
        }
    }
}