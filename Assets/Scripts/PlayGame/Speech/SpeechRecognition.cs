using System.Text.RegularExpressions;
using Assets.Scripts.PlayGame;
using PlayGame.Movement;
using UnityEngine;
using UnityEngine.UI;
using Ping = Assets.Scripts.PlayGame.Ping;

namespace PlayGame.Speech
{
    public class SpeechRecognition : MonoBehaviour {
        public Text text;
        private string _myResponse = "...";

        public GameObject player;
        private MoveObject _moveObject;
        private PlayerData _playerData;
    
        public GameObject ping;
        private PingManager _pingManager;
    
        private void Start() {
            _moveObject = player.GetComponent<MoveObject>();
            _playerData = player.GetComponent<PlayerData>();
            _pingManager = ping.GetComponent<PingManager>();
        }

        private void Update() {
            text.text = _myResponse;
        }

        // Called by javascript when speech is detected
        public void GetResponse(string result) {
            _myResponse = result.ToLower();
            PerformActions(_myResponse);
        }

        private static bool IsMovementCommand(string phrase) {
            return phrase.Contains("move") || phrase.Contains("face") || phrase.Contains("go");
        }

        // Returns the direction vector or null
        private static Vector3? GetDirection(string phrase) {
            if (phrase.Contains("north")) {
                return Vector3.forward;
            }

            if (phrase.Contains("south")) {
                return Vector3.back;
            }

            if (phrase.Contains("west")) {
                return Vector3.left;
            }

            if (phrase.Contains("east")) {
                return Vector3.right;
            }

            return null;
        }

        // Gets a grid coordinate from the input phrase, if no grid coordinate is found it returns null
        private static GridCoord? GetGridPosition(string phrase) {
            Match coordMatch = Regex.Match(phrase, @"[a-z]( )?(\d+)"); // Letter Number with optional space
        
            if (!coordMatch.Success) return null;
        
            Match number = Regex.Match(coordMatch.Value, @"(\d+)"); // One or more numbers
            char x = coordMatch.Value[0];
            int z = int.Parse(number.Value);

            return new GridCoord(x, z);
        }

        private static PingType? GetPingType(string phrase) {
            if (phrase.Contains("none")) return PingType.None;
            if (phrase.Contains("asteroid")) return PingType.Asteroid;
            if (phrase.Contains("pirate")) return PingType.Pirate;

            return null;
        }

        private static Ping? GetPing(string phrase) {
            PingType? type = GetPingType(phrase);
            if (type == null) return null;

            GridCoord? gridCoord = GetGridPosition(phrase);
            if (gridCoord == null) return null;

            return new Ping((GridCoord) gridCoord, (PingType) type);
        }
    
        private void PerformMovement(string phrase) {
            // Check if phrase contains a direction
            Vector3? direction = GetDirection(phrase);
            if (direction != null) {
                _moveObject.SetDirection((Vector3) direction);
                return;
            }
        
            // Check if phrase contains a grid coordinate
            GridCoord? position = GetGridPosition(phrase);
            if (position != null)
            {
                GridCoord coord = (GridCoord) position;
                _moveObject.SetDestination(coord.GetWorldVector());
                return;
            }
        
            // Check if phrase contains ping
            if (phrase.Contains("ping")) {
                if (_pingManager.GetPing().GetPingType() != PingType.None) { // Only move to ping if theres an active ping
                    _moveObject.SetDestination(_pingManager.GetPing().GetPositionVector());
                }
            }
        }
    
        private void PerformActions(string phrase) {
            if (IsMovementCommand(phrase)) {
                PerformMovement(phrase);
            } else if (phrase.Contains("ping")) { // Check for create/delete ping commands
                if (phrase.Contains("remove") || phrase.Contains("delete")) {
                    _pingManager.SetPing('A', 0, PingType.None);
                } else {
                    if (_playerData.GetRole() == Role.StationCommander) { // Only let the station commander create pings
                        Ping? newPing = GetPing(phrase);
                        if (newPing != null) _pingManager.SetPing((Ping) newPing);
                    }
                }
            }

            if (phrase.Contains("stop")) {
                _moveObject.SetSpeed(0);
            }

            if (phrase.Contains("go") || phrase.Contains("move")) {
                _moveObject.SetSpeed(1);
            }
        }

        public void StartSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("startButtonFromUnity3D");
        }

    }
}
