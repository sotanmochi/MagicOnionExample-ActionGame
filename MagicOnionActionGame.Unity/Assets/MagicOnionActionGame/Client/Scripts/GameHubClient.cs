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
        IGameHub _streamingHub;

        void Awake()
        {
            MagicOnionNetwork.RegisterHubClient(this);
        }

        void Start()
        {
            Debug.Log("*** Start @GameHubClient ***");
            Debug.Log("Connected: " + MagicOnionNetwork.IsConnected);
            Debug.Log("State: " + MagicOnionNetwork.ConnectionState);
        }

        void IGameHubReceiver.OnJoin(Player player)
        {
            Debug.Log("OnJoin - Player[" + player.ActorNumber + "]: " + player.Name);
        }

        void IGameHubReceiver.OnLeave(Player player)
        {
            Debug.Log("OnLeave - Player[" + player.ActorNumber + "]:" + player.Name);
        }

        void IHubClient.ConnectHub(Channel channel)
        {
            _streamingHub = StreamingHubClient.Connect<IGameHub, IGameHubReceiver>(channel, this);
        }

        async Task IHubClient.DisconnectHubAsync()
        {
            await _streamingHub.DisposeAsync();
        }

        Task<JoinResult> IHubClient.JoinHubAsync(string roomName, string playerName, string userId)
        {
            return _streamingHub.JoinAsync(roomName, playerName, userId);
        }

        async void IHubClient.LeaveHubAsync()
        {
            await _streamingHub.LeaveAsync();
        }
    }
}
