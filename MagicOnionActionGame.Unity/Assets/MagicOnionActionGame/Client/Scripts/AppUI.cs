using UnityEngine;
using UnityEngine.UI;

namespace MagicOnionExample.ActionGame.Client
{
    public class AppUI : MonoBehaviour
    {
        [SerializeField] GameHubClient Client;

        [SerializeField] InputField RoomName;
        [SerializeField] InputField PlayerName;
        [SerializeField] Button JoinButton;
        [SerializeField] Button LeaveButton;

        void Start()
        {
            JoinButton.onClick.AddListener(OnJoinClicked);
            LeaveButton.onClick.AddListener(OnLeaveClicked);
        }

        async void OnJoinClicked()
        {
            await Client.JoinAsync(RoomName.text, PlayerName.text);
        }

        void OnLeaveClicked()
        {
            Client.LeaveAsync();
        }
    }
}