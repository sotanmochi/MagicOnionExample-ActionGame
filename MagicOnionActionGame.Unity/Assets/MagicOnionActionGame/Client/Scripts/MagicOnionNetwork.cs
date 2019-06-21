using Grpc.Core;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MagicOnionExample
{
    public interface IHubClient
    {
        void Connect(Channel channel);
        Task DisconnectAsync();
    }

    public class MagicOnionNetwork
    {
        public static Player LocalPlayer;

        public static bool IsConnected
        {
            get
            {
                return (_channel.State == ChannelState.Ready) ? true : false;
            }
        }

        public static ChannelState ConnectionState
        {
            get
            {
                return _channel.State;
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
            _channel = new Channel(host, port, credentials);
            foreach (IHubClient hubClient in _hubClientSet)
            {
                hubClient.Connect(_channel);
            }
        }

        public static async void DisconnectAsync()
        {
            List<Task> taskList = new List<Task>();
            foreach (IHubClient hubClient in _hubClientSet)
            {
                taskList.Add(Task.Run(() =>
                {
                    hubClient.DisconnectAsync();
                }));
            }
            await Task.WhenAll(taskList);

            _hubClientSet.Clear();

            await _channel.ShutdownAsync();
        }

        public static bool RegisterHubClientAsync(IHubClient client)
        {
            bool result = _hubClientSet.Add(client);
            if (result && (_channel != null))
            {
                client.Connect(_channel);
            }
            return result;
        }

        public static async Task UnregisterHubClientAsync(IHubClient client)
        {
            if (_hubClientSet.Remove(client))
            {
                await client.DisconnectAsync();
            }
        }
    }
}
