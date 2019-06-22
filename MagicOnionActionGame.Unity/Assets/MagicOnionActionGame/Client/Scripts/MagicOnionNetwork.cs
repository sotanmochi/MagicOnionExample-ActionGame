using Grpc.Core;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MagicOnionExample
{
    public interface IHubClient
    {
        void ConnectHub(Channel channel);
        Task DisconnectHubAsync();
        Task<JoinResult> JoinHubAsync(string roomName, string playerName, string userId);
        void LeaveHubAsync();
    }

    public class MagicOnionNetwork
    {
        public static Player LocalPlayer;

        public static bool IsConnected
        {
            get
            {
                return ((_channel != null) && (_channel.State == ChannelState.Ready)) ? true : false;
            }
        }

        public static ChannelState ConnectionState
        {
            get
            {
                return (_channel != null) ? _channel.State : ChannelState.Idle;
            }
        }

        private static Channel _channel;
        private static HashSet<IHubClient> _hubClientSet;

        /// <summary>
        /// Static constructor used for basic setup.
        /// </summary>
        static MagicOnionNetwork()
        {
            _hubClientSet = new HashSet<IHubClient>();

            GameObject connectionHandlerGameObject = new GameObject();
            connectionHandlerGameObject.name = "MagicOnionConnectionHandler";
            connectionHandlerGameObject.AddComponent<MagicOnionConnectionHandler>();
        }

        public static void Connect(string host, int port)
        {
            Connect(host, port, ChannelCredentials.Insecure);
        }

        public static void Connect(string host, int port, ChannelCredentials credentials)
        {
            if (!IsConnected)
            {
                _channel = new Channel(host, port, credentials);
                foreach (IHubClient hubClient in _hubClientSet)
                {
                    hubClient.ConnectHub(_channel);
                }
            }
        }

        public static async void DisconnectAsync()
        {
            List<Task> taskList = new List<Task>();
            foreach (IHubClient hubClient in _hubClientSet)
            {
                taskList.Add(Task.Run(() =>
                {
                    hubClient.DisconnectHubAsync();
                }));
            }
            await Task.WhenAll(taskList);

            _hubClientSet.Clear();

            if (_channel != null)
            {
                await _channel.ShutdownAsync();
            }
        }

        public static bool RegisterHubClient(IHubClient client)
        {
            bool result = _hubClientSet.Add(client);
            if (result && (_channel != null))
            {
                client.ConnectHub(_channel);
            }
            return result;
        }

        public static async Task UnregisterHubClientAsync(IHubClient client)
        {
            if (_hubClientSet.Remove(client))
            {
                await client.DisconnectHubAsync();
            }
        }

        public static async Task<bool> JoinAsync(string roomName, string playerName, string userId)
        {
            if (!IsConnected)
            {
                Debug.Log("Channel is not ready!!");
                return false;
            }

            if (LocalPlayer != null && LocalPlayer.ActorNumber >= 0)
            {
                Debug.Log("Already joined!!");
                Debug.Log("LocalPlayer.ActorNumber: " + LocalPlayer.ActorNumber);
                Debug.Log("LocalPlayer.UserId: " + LocalPlayer.UserId);
            }
            else
            {
                List<Task<JoinResult>> taskList = new List<Task<JoinResult>>();
                foreach (IHubClient hubClient in _hubClientSet)
                {
                    taskList.Add(Task<JoinResult>.Run<JoinResult>(() =>
                    {
                        return hubClient.JoinHubAsync(roomName, playerName, userId);
                    }));
                }
                JoinResult[] result = await Task.WhenAll(taskList);

                Debug.Log("Num of JoinResults: " + result.Length);
                if (result.Length > 0)
                {
                    JoinResult joinResult = result[0];

                    Debug.Log("RoomPlayers: " + joinResult.RoomPlayers.Length);

                    if (joinResult.LocalPlayer.ActorNumber >= 0)
                    {
                        LocalPlayer = joinResult.LocalPlayer;
                        Debug.Log("LocalPlayer.ActorNumber: " + joinResult.LocalPlayer.ActorNumber);
                    }
                }
            }

            return true;
        }

        public static async void LeaveAsync()
        {
            List<Task> taskList = new List<Task>();
            foreach (IHubClient hubClient in _hubClientSet)
            {
                taskList.Add(Task.Run(() =>
                {
                    hubClient.LeaveHubAsync();
                }));
            }
            await Task.WhenAll(taskList);

            MagicOnionNetwork.LocalPlayer = null;
        }
    }
}
