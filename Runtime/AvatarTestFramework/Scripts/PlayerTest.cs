using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
namespace RLTY.Customisation.Testing
{
    public class PlayerTest : MonoBehaviour, IPlayer
    {
        public Transform Transform => transform;

        public int ClientId => 0;

        public int IsTalking { get; set; }
        public bool Sync_IsAdmin { get => false; set{ } }
        public uint AgoraUserId { get => 0; set { } }
        public bool Sync_IsVoiceBoosted { get; set; }
        public bool Sync_IsServerMuted { get; set; }
        public bool Sync_IsCrossServerMuted { get; set; }
        public bool IsLocalyMuted { get; set; }
        public string Username { get => "TestPlayer"; set { } }
        public string SkinDesc { get => null; set { } }

        public event Action<int> OnTalkChanged;
        public event Action<string> OnNameChanged;
        public event Action<string> OnSkinRebuild;

        void Start()
        {
            Physics.autoSimulation = true;
            Physics.autoSyncTransforms = true;
            AllPlayers.List = new Dictionary<int, IPlayer>();
            AllPlayers.NotifyPlayerJoined(this, true);
        }

        /*
        public bool sim, sim2;
        void Update()
        {
            sim = Physics.autoSimulation;
            sim2 = Physics.autoSyncTransforms;
        }*/

        void OnDestroy()
        {
            AllPlayers.NotifyPlayerLeft(this);
        }

        protected PlayerControllerForTesting PlayerController
        {
            get
            {
                if (_playerController == null)
                    _playerController = GetComponent<PlayerControllerForTesting>();
                return _playerController;
            }
        }
        private PlayerControllerForTesting _playerController;

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            PlayerController.Teleport(position, rotation);
        }

        public void SetAdditionalSpeed(Vector3 worldSpaceSpeedVector)
        {
            PlayerController.SetAdditionalSpeed(worldSpaceSpeedVector);
        }

        public void SetVerticalVelocity(float verticalVelocity)
        {
            PlayerController.SetVerticalVelocity(verticalVelocity);
        }
    }
}
#endif
