using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using UnityEngine;

namespace MagicOnionExample.ActionGame.Client
{
    public class GameHubComponent : MonoBehaviour, IGameHubReceiver
    {
        public static GameHubComponent Instance { get { return _instance; } }

        private static GameHubComponent _instance;
        private GameHubClient _gameHubClient;

        void Awake()
        {
            _instance = this;
            _gameHubClient = new GameHubClient(this);
            MagicOnionNetwork.RegisterHubClient(_gameHubClient);
        }

        void IGameHubReceiver.OnJoin(Player player)
        {
            Debug.Log("OnJoin@GameHub - Player[" + player.ActorNumber + "]: " + player.Name);
        }

        void IGameHubReceiver.OnLeave(Player player)
        {
            Debug.Log("OnLeave@GameHub - Player[" + player.ActorNumber + "]:" + player.Name);
        }
    }
}
