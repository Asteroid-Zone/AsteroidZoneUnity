using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using Statics;
using System.Collections.Generic;
using Photon.GameControllers;
using Photon.Pun;
using PlayGame.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.Speech {
    public class SpeechRecognition : MonoBehaviourPun {
        
        public Text text;
        private string _myResponse = "...";

        public GameObject player;
        public GameObject spaceStation;
        public GameObject ping;
        public GameObject cameraManager;
        
        private ActionController _actionController;

        private void Start() {
            StartSpeechRecognitionInTheBrowser();
            if (!DebugSettings.Debug) player = PhotonPlayer.Instance.myAvatar;

            MovementCommand.player = player.transform;

            _actionController = CreateActionController(player);
        }

        private ActionController CreateActionController(GameObject player) {
            _actionController = new ActionController {
                speechRecognition = this,
                player = player,
                spaceStationObject = spaceStation,
                moveObject = player.GetComponent<MoveObject>(),
                miningLaser = player.GetComponent<MiningLaser>(),
                laserGun = player.GetComponent<LaserGun>(),
                playerData = player.GetComponent<PlayerData>(),
                pingManager = ping.GetComponent<PingManager>(),
                spaceStation = spaceStation.GetComponent<SpaceStation>(),
                cameraFollow = cameraManager.GetComponent<CameraManager>().followCamera.GetComponent<CameraFollow>()
            };
            return _actionController;
        }

        public void StartLockOn(Transform lockTarget) {
            StartCoroutine(_actionController.LockOn(lockTarget));
        }

        public void StopLockOn(Transform lockTarget) {
            StopCoroutine(_actionController.LockOn(lockTarget));
        }

        private void Update() {
            text.text = _myResponse;
        }

        private void OnDestroy() {
            StopSpeechRecognitionInTheBrowser();
        }

        // Called by javascript when speech is detected
        public void GetResponse(string result) {
            if (!DebugSettings.Debug && !PhotonPlayer.Instance.photonView.IsMine) return;
            
            _myResponse = result.ToLower();
            Command command = Grammar.GetCommand(_myResponse);
            
            if (command.IsValid()) {
                if (DebugSettings.Debug) _actionController.PerformActions(command);
                else photonView.RPC("RPC_PerformActions", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, _myResponse);
            }
        }

        [PunRPC]
        public void RPC_PerformActions(int viewID, string _myResponse) {
            GameObject prev_player = player;
            Command command = Grammar.GetCommand(_myResponse);
            List<GameObject> playerList = player.GetComponent<PlayerData>().GetList();
            foreach (GameObject _player in playerList) {
                if (viewID == _player.GetComponent<PhotonView>().ViewID) player = _player;
            }

            _actionController = CreateActionController(player);
            _actionController.PerformActions(command);
            player = prev_player;
        }

        private static void StartSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("startVoiceRecognition");
        }
        
        private static void StopSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("stopVoiceRecognition");
        }

    }
}
