using PhotonClass.GameController;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.Speech {
    public class SpeechRecognition : MonoBehaviour {
        
        public Text text;
        private string _myResponse = "...";

        public GameObject player;
        public GameObject spaceStation;
        public GameObject ping;

        private ActionController _actionController;
    
        private void Start() {
            StartSpeechRecognitionInTheBrowser();
            if (!Variables.Debug) player = PhotonPlayer.PP.myAvatar;

            _actionController = new ActionController {
                speechRecognition = this,
                player = player,
                spaceStationObject = spaceStation,
                moveObject = player.GetComponent<MoveObject>(),
                miningLaser = player.GetComponent<MiningLaser>(),
                laserGun = player.GetComponent<LaserGun>(),
                playerData = player.GetComponent<PlayerData>(),
                pingManager = ping.GetComponent<PingManager>(),
                spaceStationCollider = spaceStation.GetComponent<Collider>(),
                spaceStation = spaceStation.GetComponent<SpaceStation>()
            };
        }

        public void StartLockOn(Transform lockTarget) {
            StartCoroutine(_actionController.LockOn(lockTarget));
        }

        public void StopLockOn() {
            StopCoroutine(_actionController.LockOn(null));
        }

        private void Update() {
            text.text = _myResponse;
        }

        private void OnDestroy() {
            StopSpeechRecognitionInTheBrowser();
        }

        // Called by javascript when speech is detected
        public void GetResponse(string result) {
            _myResponse = result.ToLower();
            Command command = Grammar.GetCommand(_myResponse);
            if (command.IsValid()) _actionController.PerformActions(command);
        }

        private static void StartSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("startVoiceRecognition");
        }
        
        private static void StopSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("stopVoiceRecognition");
        }

    }
}
