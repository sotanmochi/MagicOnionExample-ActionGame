using UnityEngine;
using UnityEngine.UI;

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

        void Start()
        {
            ConnectButton.onClick.AddListener(OnConnectClicked);
            JoinButton.onClick.AddListener(OnJoinClicked);
            LeaveButton.onClick.AddListener(OnLeaveClicked);
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
    }
}