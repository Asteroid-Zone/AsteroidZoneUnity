using PhotonClass.GameController;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
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
            player = PhotonPlayer.PP.myAvatar;

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

        public void StartLockOn(GameObject lockTarget) {
            StartCoroutine(_actionController.LockOn(lockTarget));
        }

        public void StopLockOn(GameObject lockTarget) {
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
            _myResponse = result.ToLower();
            Command command = Grammar.GetCommand(_myResponse);
            if (command.IsValid()) _actionController.PerformActions(command);
        }

        // Returns the direction vector or null
        /*private static Vector3? GetDirection(string phrase) {
            if (phrase.Contains("north")) return Vector3.forward;
            if (phrase.Contains("south")) return Vector3.back;
            if (phrase.Contains("west")) return Vector3.left;
            if (phrase.Contains("east")) return Vector3.right;

            return null;
        }*/

        // Gets a grid coordinate from the input phrase, if no grid coordinate is found it returns null
        /*private static GridCoord? GetGridPosition(string phrase) {
            Match coordMatch = Regex.Match(phrase, @"[a-z]( )?(\d+)"); // Letter followed by one or more numbers with an optional space
        
            if (!coordMatch.Success) return null;
        
            Match number = Regex.Match(coordMatch.Value, @"(\d+)"); // One or more numbers
            char x = coordMatch.Value[0];
            int z = int.Parse(number.Value);

            return new GridCoord(x, z);
        }*/

        /*private void PerformMovement(string phrase) {
            // Check if phrase contains ping
            if (phrase.Contains("ping")) {
                // Check whether there is only one ping and if so go to the ping
                // TODO: somehow number pings so that the player can go to a specific one
                var pings = _pingManager.GetPings();
                if (pings.Count == 1)
                {
                    var onlyPing = pings.Keys.ToList()[0];
                    if(onlyPing.GetPingType() != PingType.None) { // Only move to ping if theres an active ping
                        _moveObject.SetDestination(onlyPing.GetPositionVector());
                    }
                }
            }
        }*/
        

        private static void StartSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("startVoiceRecognition");
        }
        
        private static void StopSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("stopVoiceRecognition");
        }

    }
}
