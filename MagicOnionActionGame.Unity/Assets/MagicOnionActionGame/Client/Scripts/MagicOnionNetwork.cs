using Grpc.Core;
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
        public static string Host = "localhost";
        public static int Port = 12345;

        public static int LocalPlayerId = -1;

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

        public static void Connect()
        {
            _channel = new Channel(Host, Port, ChannelCredentials.Insecure);
            foreach (IHubClient hubClient in _hubClientSet)
            {
                hubClient.Connect(_channel);
            }
        }

        public static async void DisconnectAsync()
        {
            foreach (IHubClient hubClient in _hubClientSet)
            {
                await hubClient.DisconnectAsync();
            }
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
