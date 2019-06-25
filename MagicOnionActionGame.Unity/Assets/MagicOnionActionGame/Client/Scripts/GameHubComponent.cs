using MagicOnionExample.ActionGame.ServerShared.Hubs;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using System;
using UnityEngine;

namespace MagicOnionExample.ActionGame.Client
{
    public class GameHubComponent : MonoBehaviour, IGameHubReceiver
    {
        public static GameHubComponent Instance { get { return _instance; } }

        public delegate void OnMoveHandler(PlayerCharacterParameter param);
        public OnMoveHandler OnMovePlayerCharacter;

        public delegate void OnJoinOrLeaveHandler(Player player);
        public OnJoinOrLeaveHandler OnJoin;
        public OnJoinOrLeaveHandler OnLeave;

        public Action AfterJoinGameHub;
        public Action BeforeLeaveGameHub;
        public Action AfterLeaveGameHub;

        private static GameHubComponent _instance;
        private GameHubClient _gameHubClient;

        void Awake()
        {
            _instance = this;

            _gameHubClient = new GameHubClient(this);
            _gameHubClient.AfterJoinHub += () => AfterJoinGameHub?.Invoke();
            _gameHubClient.BeforeLeaveHub += () => BeforeLeaveGameHub?.Invoke();
            _gameHubClient.AfterLeaveHub += () => AfterLeaveGameHub?.Invoke();

            MagicOnionNetwork.RegisterHubClient(_gameHubClient);
        }

        public void MoveAsync(PlayerCharacterParameter param)
        {
            if (MagicOnionNetwork.IsConnected)
            {
                _gameHubClient.MoveAsync(param);
            }
        }

        void IGameHubReceiver.OnMove(PlayerCharacterParameter param)
        {
            OnMovePlayerCharacter?.Invoke(param);
        }

        void IGameHubReceiver.OnJoin(Player player)
        {
            OnJoin?.Invoke(player);
        }

        void IGameHubReceiver.OnLeave(Player player)
        {
            OnLeave?.Invoke(player);
        }
    }
}
