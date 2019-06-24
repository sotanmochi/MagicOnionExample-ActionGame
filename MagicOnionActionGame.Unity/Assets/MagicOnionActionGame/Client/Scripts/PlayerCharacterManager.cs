using System.Collections.Generic;
using MagicOnionExample.ActionGame.ServerShared.MessagePackObjects;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace MagicOnionExample.ActionGame.Client
{
    public class PlayerCharacterManager : MonoBehaviour
    {
        public static PlayerCharacterManager Instance { get { return _instance; } }

        [SerializeField]
        private GameObject LocalPlayerObject;
        [SerializeField]
        private GameObject PlayerPrefab;
        [SerializeField]
        private GameObject RemotePlayerRoot;

        private Dictionary<int, ThirdPersonCharacter> _remotePlayers;
        private static PlayerCharacterManager _instance;

        void Awake()
        {
            _instance = this;
            _remotePlayers = new Dictionary<int, ThirdPersonCharacter>();
        }

        void Start()
        {
            GameHubComponent.Instance.AfterJoinGameHub += AfterJoinGameHub;
            GameHubComponent.Instance.BeforeLeaveGameHub += BeforeLeaveGameHub;
            GameHubComponent.Instance.OnMovePlayerCharacter += OnMoveRemotePlayerCharacter;
            GameHubComponent.Instance.OnLeave += OnLeave;

            var controller = LocalPlayerObject.GetComponent<ModifiedThirdPersonUserControl>();
            if (controller != null)
            {
                controller.OnUpdateCharacterMove += OnUpdateLocalPlayerMove;
            }
        }

        private void OnUpdateLocalPlayerMove(Vector3 position, Quaternion rotation, Vector3 move, bool crouch, bool jump)
        {
            if (MagicOnionNetwork.IsJoined)
            {
                PlayerCharacterParameter param = new PlayerCharacterParameter();
                param.ActorNumber = MagicOnionNetwork.LocalPlayer.ActorNumber;
                param.PlayerName = MagicOnionNetwork.LocalPlayer.Name;
                param.Position = position;
                param.Rotation = rotation;
                param.Move = move;
                param.Crouch = crouch;
                param.Jump = jump;

                GameHubComponent.Instance.MoveAsync(param);
            }
        }

        private void OnMoveRemotePlayerCharacter(PlayerCharacterParameter param)
        {
            ThirdPersonCharacter remotePlayer;
            _remotePlayers.TryGetValue(param.ActorNumber, out remotePlayer);
            if (remotePlayer != null)
            {
                remotePlayer.transform.position = param.Position;
                remotePlayer.transform.rotation = param.Rotation;
                remotePlayer.Move(param.Move, param.Crouch, param.Jump);
            }
            else
            {
                GameObject go = GameObject.Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(RemotePlayerRoot.transform);
                go.name = "RemotePlayer[" + param.ActorNumber + "]_" + param.PlayerName;

                _remotePlayers[param.ActorNumber] = go.GetComponent<ThirdPersonCharacter>();
            }
        }

        private void OnLeave(Player player)
        {
            ThirdPersonCharacter remotePlayer;
            _remotePlayers.TryGetValue(player.ActorNumber, out remotePlayer);
            if (remotePlayer != null)
            {
                DestroyImmediate(remotePlayer.gameObject);
                _remotePlayers.Remove(player.ActorNumber);
            }
        }

        private void AfterJoinGameHub()
        {
            PlayerCharacterParameter param = new PlayerCharacterParameter();
            param.ActorNumber = MagicOnionNetwork.LocalPlayer.ActorNumber;
            param.PlayerName = MagicOnionNetwork.LocalPlayer.Name;

            GameHubComponent.Instance.MoveAsync(param);
        }

        private void BeforeLeaveGameHub()
        {
            // foreach(var remotePlayer in _remotePlayers.Values)
            // {
            //     DestroyImmediate(remotePlayer.gameObject);
            // }
            // _remotePlayers.Clear();
        }
    }
}
