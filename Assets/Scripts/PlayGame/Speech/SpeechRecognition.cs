using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PhotonClass.GameController;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using UnityEngine;
using UnityEngine.UI;
using Ping = PlayGame.Pings.Ping;

namespace PlayGame.Speech
{
    public class SpeechRecognition : MonoBehaviour {
        public Text text;
        private string _myResponse = "...";

        public GameObject player;
        public GameObject spaceStation;
        private Collider _spaceStationCollider;
        private MoveObject _moveObject;
        private MiningLaser _miningLaser;
        private LaserGun _laserGun;
        private PlayerData _playerData;
        private SpaceStation _spaceStation;
    
        public GameObject ping;
        private PingManager _pingManager;

        private bool _lockedOn = false;
    
        private void Start() {
            StartSpeechRecognitionInTheBrowser();
            player = PhotonPlayer.PP.myAvatar;
            _moveObject = player.GetComponent<MoveObject>();
            _miningLaser = player.GetComponent<MiningLaser>();
            _laserGun = player.GetComponent<LaserGun>();
            if (_laserGun == null) Debug.Log("Laser Gun is Null!");
            _playerData = player.GetComponent<PlayerData>();
            _pingManager = ping.GetComponent<PingManager>();
            _spaceStationCollider = spaceStation.GetComponent<Collider>();
            _spaceStation = spaceStation.GetComponent<SpaceStation>();
        }

        private void Update() {
            text.text = _myResponse;
        }

        private void OnDestroy()
        {
            StopSpeechRecognitionInTheBrowser();
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
            if (phrase.Contains("north")) return Vector3.forward;
            if (phrase.Contains("south")) return Vector3.back;
            if (phrase.Contains("west")) return Vector3.left;
            if (phrase.Contains("east")) return Vector3.right;

            return null;
        }

        // Gets a grid coordinate from the input phrase, if no grid coordinate is found it returns null
        private static GridCoord? GetGridPosition(string phrase) {
            Match coordMatch = Regex.Match(phrase, @"[a-z]( )?(\d+)"); // Letter followed by one or more numbers with an optional space
        
            if (!coordMatch.Success) return null;
        
            Match number = Regex.Match(coordMatch.Value, @"(\d+)"); // One or more numbers
            char x = coordMatch.Value[0];
            int z = int.Parse(number.Value);

            return new GridCoord(x, z);
        }

        // Gets the ping type from the input phrase
        private static PingType? GetPingType(string phrase) {
            if (phrase.Contains("none")) return PingType.None;
            if (phrase.Contains("asteroid")) return PingType.Asteroid;
            if (phrase.Contains("pirate")) return PingType.Pirate;

            return null;
        }

        // Gets a Ping from the input phrase
        private static Ping GetPing(string phrase) {
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
            
            // Check if phrase contains station
            if (phrase.Contains("station")) {
                _moveObject.SetDestination(spaceStation.transform.position, _spaceStationCollider);
            }
        }
        
        IEnumerator LockOn()
        {
            while (_lockedOn)
            {
                Transform transform = player.GetComponent<MoveObject>().GetNearestEnemyTransform();
                player.GetComponent<MoveObject>().FaceTarget(transform);
                yield return null;
            }
        }
    
        private void PerformActions(string phrase) {
            if (IsMovementCommand(phrase)) {
                PerformMovement(phrase);
                // TODO change back to ping when speech recognition is improved (currently ping isn't detected properly)
            } else if (phrase.Contains("pin") && _playerData.GetRole() == Role.StationCommander) { // Check for create ping commands
                // Note pin only is used because listener often hears pink or pin instead of ping
                // Only let the station commander create pings
                Ping newPing = GetPing(phrase);
                if (newPing != null) _pingManager.AddPing(newPing);
            }

            if (phrase.Contains("stop")) {
                _moveObject.SetSpeed(0);
            }

            if (phrase.Contains("go") || phrase.Contains("move")) {
                _moveObject.SetSpeed(1);
            }

            if (phrase.Contains("deactivate mining laser")) {
                _miningLaser.DisableMiningLaser();
            } else if (phrase.Contains("activate mining laser")) {
                _miningLaser.EnableMiningLaser();
            }

            if (phrase.Contains("transfer resources")) {
                // Check player is in the same grid square as the station
                if (GridCoord.GetCoordFromVector(player.transform.position).Equals(GridCoord.GetCoordFromVector(spaceStation.transform.position))) {
                    _spaceStation.AddResources(_playerData.GetResources()); // Add the resources into the space station
                    _playerData.RemoveResources(); // Remove them from the player
                } else {
                    EventsManager.AddMessageToQueue("You must be next to the space station to transfer resources");
                }
            }

            if (phrase.Contains("stop shooting")) {
                _laserGun.StopShooting();
            } else if (phrase.Contains("shoot") || phrase.Contains("fire")) {
                _laserGun.StartShooting();
            }

            if (phrase.Contains("lock on nearest enemy"))
            {
                _lockedOn = true;
                StartCoroutine(LockOn());
            }

            if (phrase.Contains("disengage lock on"))
            {
                _lockedOn = false;
                StopCoroutine(LockOn());
            }
        }

        private static void StartSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("startVoiceRecognition");
        }
        
        private static void StopSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("stopVoiceRecognition");
        }

    }
}
