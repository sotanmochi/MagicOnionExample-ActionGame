using Grpc.Core;
using MagicOnion.Client;
using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using System.Threading.Tasks;
using UnityEngine;

namespace MagicOnionExample.ActionGame.Client
{
    public class GameHubClient : MonoBehaviour, IHubClient, IGameHubReceiver
    {
        IGameHub _hubClient;

        void Awake()
        {
            MagicOnionNetwork.RegisterHubClientAsync(this);
            MagicOnionNetwork.Connect("localhost", 12345);
        }

        void Start()
        {
            Debug.Log("*** Start @GameHubClient ***");
            Debug.Log("Connected: " + MagicOnionNetwork.IsConnected);
            Debug.Log("State: " + MagicOnionNetwork.ConnectionState);
        }

        #region Client -> Server (Streaming)

        public async Task<int> JoinAsync(string roomName, string playerName)
        {
            JoinResult result = await _hubClient.JoinAsync(roomName, playerName);
            if (result.LocalPlayer.ActorNumber >= 0)
            {
                MagicOnionNetwork.LocalPlayer = result.LocalPlayer;
                Debug.Log("Local player id: " + result.LocalPlayer.ActorNumber);
            }

            Debug.Log("RoomPlayers: " + result.RoomPlayers.Length);

            return result.LocalPlayer.ActorNumber;
        }

        public async void LeaveAsync()
        {
            await _hubClient.LeaveAsync();
            Debug.Log("LeaveAsync()");
        }

        #endregion

        #region Server -> Client (Streaming)

        void IGameHubReceiver.OnJoin(Player player)
        {
            Debug.Log("OnJoin - Player[" + player.ActorNumber + "]: " + player.Name);
        }

        void IGameHubReceiver.OnLeave(Player player)
        {
            Debug.Log("OnLeave - Player[" + player.ActorNumber + "]:" + player.Name);
        }

        #endregion

        void IHubClient.Connect(Channel channel)
        {
            _hubClient = StreamingHubClient.Connect<IGameHub, IGameHubReceiver>(channel, this);
        }

        async Task IHubClient.DisconnectAsync()
        {
            await _hubClient.DisposeAsync();
        }
    }
}
