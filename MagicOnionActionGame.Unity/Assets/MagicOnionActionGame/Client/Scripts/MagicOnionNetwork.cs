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
        void AfterJoinHub();
        void BeforeLeaveHub();
        void AfterLeaveHub();
    }

    public class MagicOnionNetwork
    {
        public static Player LocalPlayer { get { return _localPlayer; } }
        private static Player _localPlayer;

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

        public static bool IsJoined { get { return _isJoined; } }
        private static bool _isJoined;

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

        public static async Task DisconnectAsync()
        {
            Debug.Log("DisconnectAsync Start");

            // Leave hubs
            await LeaveAsync();
            Debug.Log("LeaveAsync has finished @DisconnectAsync");

            // Disconnect hubs
            List<Task> taskList = new List<Task>();
            foreach (IHubClient hubClient in _hubClientSet)
            {
                taskList.Add(Task.Run(() =>
                {
                    hubClient.DisconnectHubAsync();
                }));
            }
            await Task.WhenAll(taskList);

            Debug.Log("DisconnectHubAsync End");
            _hubClientSet.Clear();

            if (_channel != null)
            {
                await _channel.ShutdownAsync();
            }

            Debug.Log("DisconnectAsync End");
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

            if (IsJoined && LocalPlayer != null && LocalPlayer.ActorNumber >= 0)
            {
                Debug.Log("Already joined!!");
                Debug.Log("LocalPlayer.ActorNumber: " + LocalPlayer.ActorNumber);
            }
            else
            {
                // Join hubs
                List<Task<JoinResult>> taskList = new List<Task<JoinResult>>();
                foreach (IHubClient hubClient in _hubClientSet)
                {
                    taskList.Add(Task<JoinResult>.Run<JoinResult>(() =>
                    {
                        return hubClient.JoinHubAsync(roomName, playerName, userId);
                    }));
                }
                JoinResult[] result = await Task.WhenAll(taskList);

                if (result.Length > 0)
                {
                    JoinResult joinResult = result[0];

                    if (joinResult.LocalPlayer.ActorNumber >= 0)
                    {
                        _localPlayer = joinResult.LocalPlayer;
                        _isJoined = true;
                    }
                    else
                    {
                        return false;
                    }

                    // After join hubs
                    List<Task> afterTaskList = new List<Task>();
                    foreach (IHubClient hubClient in _hubClientSet)
                    {
                        afterTaskList.Add(Task.Run(() =>
                        {
                            hubClient.AfterJoinHub();
                        }));
                    }
                    await Task.WhenAll(afterTaskList);
                }
            }

            return true;
        }

        public static async Task LeaveAsync()
        {
            Debug.Log("LeaveAsync Start");

            if (IsJoined && LocalPlayer != null)
            {
                // Before leave hubs
                List<Task> beforeTaskList = new List<Task>();
                foreach (IHubClient hubClient in _hubClientSet)
                {
                    beforeTaskList.Add(Task.Run(() =>
                    {
                        hubClient.BeforeLeaveHub();
                    }));
                }
                await Task.WhenAll(beforeTaskList);

                _isJoined = false;
                _localPlayer = null;

                // Leave hubs
                List<Task> taskList = new List<Task>();
                foreach (IHubClient hubClient in _hubClientSet)
                {
                    taskList.Add(Task.Run(() =>
                    {
                        hubClient.LeaveHubAsync();
                    }));
                }
                await Task.WhenAll(taskList);

                // After leave hubs
                List<Task> afterTaskList = new List<Task>();
                foreach (IHubClient hubClient in _hubClientSet)
                {
                    afterTaskList.Add(Task.Run(() =>
                    {
                        hubClient.AfterLeaveHub();
                    }));
                }
                await Task.WhenAll(afterTaskList);
            }

            Debug.Log("LeaveAsync End");
        }
    }
}
