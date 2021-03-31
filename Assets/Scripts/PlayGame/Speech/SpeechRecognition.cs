using System;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using Statics;
using System.Collections.Generic;
using Photon.GameControllers;
using Photon.Pun;
using PlayGame.UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PlayGame.Speech {
    public class SpeechRecognition : MonoBehaviourPun {

        public Text text;
        private string _myResponse = "...";

        public GameObject player;
        public GameObject spaceStation;
        public GameObject ping;

        private PlayerData _playerData;
        private MoveObject _moveObject;
        private MiningLaser _miningLaser;
        private LaserGun _laserGun;

        private ActionController _actionController;

        private bool _foundCommand = false;
        private List<string> _detectedPhrases = new List<string>();

        private void Start() {
            StartSpeechRecognitionInTheBrowser();
            player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();

            _playerData = player.GetComponent<PlayerData>();
            _moveObject = player.GetComponent<MoveObject>();
            _miningLaser = player.GetComponent<MiningLaser>();
            _laserGun = player.GetComponent<LaserGun>();
            text = GameObject.Find("Speech").GetComponent<Text>();
            ping = GameObject.Find("PingManager");
            spaceStation = GameObject.Find("SpaceStation");
            _actionController = CreateActionController(player);
        }

        private ActionController CreateActionController(GameObject playerObject) {
            _actionController = new ActionController {
                player = playerObject,
                spaceStationObject = spaceStation,
                moveObject = playerObject.GetComponent<MoveObject>(),
                miningLaser = playerObject.GetComponent<MiningLaser>(),
                laserGun = playerObject.GetComponent<LaserGun>(),
                playerData = playerObject.GetComponent<PlayerData>(),
                pingManager = ping.GetComponent<PingManager>(),
                spaceStation = spaceStation.GetComponent<SpaceStation.SpaceStation>(),
            };
            return _actionController;
        }

        private void Update() {
            if (GameManager.gameOver) return;
            if (!DebugSettings.Debug && !this.photonView.IsMine) return;
            text.text = _myResponse;
        }

        private void OnDestroy() {
            StopSpeechRecognitionInTheBrowser();
        }

        public void MoveStation() {
            spaceStation.transform.position = new GridCoord(Random.Range(0, 11), Random.Range(0, 11)).GetWorldVector();
        }
        
        [PunRPC]
        public void RPC_PerformActions(int viewID, string phrase)
        {
            if (player == null) return;
            GameObject prevPlayer = player;
            /*
            List<GameObject> playerList = player.GetComponent<PlayerData>().GetList();
            foreach (GameObject p in playerList) {
                if (p != null && viewID == p.GetComponent<PhotonView>().ViewID) player = p;
            }*/

            player = PhotonView.Find(viewID).gameObject;

            Command command = Grammar.GetCommand(phrase, player.GetComponent<PlayerData>(), player.transform);

            _actionController = CreateActionController(player);
            _actionController.PerformActions(command);
            player = prevPlayer;
        }
        
        private void ResetSpeechRecognition() {
            _foundCommand = false;
            _detectedPhrases.Clear();
        }
        
        // Called by javascript when speech is detected
        public void GetResponse(string result) {
            if ((!DebugSettings.Debug && !photonView.IsMine) || _foundCommand) return; // If a command has already been found for the speech return

            _myResponse = result.ToLower();
            
            _detectedPhrases.Add(_myResponse);
            Command command = Grammar.GetCommand(_myResponse, _playerData, player.transform);

            if (command.IsValid()) {
                _foundCommand = true;
                if (DebugSettings.Debug) _actionController.PerformActions(command);
                else photonView.RPC("RPC_PerformActions", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, _myResponse);
            }
        }
        
        // Called by javascript when the final speech is detected
        public void GetFinalResponse(string result) {
            if ((!DebugSettings.Debug && !photonView.IsMine) || _foundCommand) { // If a command has already been found for the speech reset and return
                ResetSpeechRecognition();
                return;
            }
            
            _myResponse = result.ToLower();
            
            _detectedPhrases.Add(_myResponse);
            Command command = Grammar.GetCommand(_myResponse, _playerData, player.transform);
            
            if (command.IsValid()) {
                _foundCommand = true;
                if (DebugSettings.Debug) _actionController.PerformActions(command);
                else photonView.RPC("RPC_PerformActions", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, _myResponse);
            } else {
                List<Tuple<string, float, bool, string>> suggestedCommands = FindSuggestedCommands(); // Tuple(command, confidence, fromData, phrase)
                Tuple<string, float, bool, string> suggestedCommand = FindBestSuggestedCommand(suggestedCommands);
                DisplaySuggestedCommand(suggestedCommand);
            }
            
            ResetSpeechRecognition();
        }

        // Displays the suggested command and performs it if we are confident its what they meant
        private void DisplaySuggestedCommand(Tuple<string, float, bool, string> suggestedCommand) {
            string eventMessage = "";
            string suggestedPhrase = suggestedCommand.Item1;
            string originalPhrase = suggestedCommand.Item4;
            float confidence = suggestedCommand.Item2;
            bool fromData = suggestedCommand.Item3;
            
            if (fromData) { // If command is from data
                if (confidence > 0.8) { // If confidence is greater than 0.8 try to perform the command
                    Command command = Grammar.GetCommand(suggestedPhrase, _playerData, player.transform);
                    
                    if (command.IsValid()) { // If command is valid perform it
                        if (DebugSettings.Debug) _actionController.PerformActions(command);
                        else photonView.RPC("RPC_PerformActions", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, suggestedPhrase);
                        eventMessage = "'" + originalPhrase + "' is an invalid command. We think you meant '" + suggestedPhrase + "' and have performed the action.";
                    } else { // If suggested command is invalid ask the user
                        eventMessage = "'" + originalPhrase + "' is an invalid command. Did you mean '" + suggestedPhrase + "' ? (confidence = " + confidence + ")";
                    }
                } else { // If confidence is less than 0.8 ask the user
                    eventMessage = "'" + originalPhrase + "' is an invalid command. Did you mean '" + suggestedPhrase + "' ? (confidence = " + confidence + ")";
                }
            } else if (confidence > 0.5) { // If confidence is greater than 0.5 for fromdistance ask the user
                eventMessage = "'" + originalPhrase + "' is an invalid command. Did you mean '" + suggestedPhrase + "' ? (confidence = " + confidence + ")";
            } else { // If we're not confident in the command tell the user
                eventMessage = "'" + originalPhrase + "' is an invalid command. We're not sure what you meant.";
            }
            
            EventsManager.AddMessage(eventMessage);
            ReadTextToSpeech(eventMessage); // Read the message using tts in the browser
        }

        private Tuple<string, float, bool, string> FindBestSuggestedCommand(List<Tuple<string, float, bool, string>> suggestedCommands) {
            Tuple<string, float, bool, string> bestCommandFromData = null;
            Tuple<string, float, bool, string> bestCommandFromDistance = null;

            foreach (Tuple<string, float, bool, string> command in suggestedCommands) {
                if (command.Item3) { // If command is from data
                    if (bestCommandFromData == null) bestCommandFromData = command;
                    else if (command.Item2 > bestCommandFromData.Item2) bestCommandFromData = command;
                } else {
                    if (bestCommandFromDistance == null) bestCommandFromDistance = command;
                    else if (command.Item2 > bestCommandFromDistance.Item2) bestCommandFromDistance = command;
                }
            }

            if (bestCommandFromData != null && bestCommandFromData.Item2 > 0) return bestCommandFromData;
            if (bestCommandFromDistance != null && bestCommandFromDistance.Item2 > 0.5) return bestCommandFromDistance;
            return null;
        }
        
        // Returns a list of suggested commands for all the detected phrases
        // Tuple(command, confidence, fromData, phrase)
        private List<Tuple<string, float, bool, string>> FindSuggestedCommands() {
            List<Tuple<string, float, bool, string>> suggestedCommands = new List<Tuple<string, float, bool, string>>();
            
            foreach (string phrase in _detectedPhrases) {
                suggestedCommands.Add(FindSuggestedCommand(phrase));
            }

            return suggestedCommands;
        }
        
        // Returns the suggested command for a given phrase
        // Tuple(command, confidence, fromData, phrase)
        private Tuple<string, float, bool, string> FindSuggestedCommand(string phrase) {
            Tuple<string, float> suggestedCommandFromData = Grammar.GetSuggestedCommandFromData(phrase, _playerData, _moveObject, _miningLaser.enabled, _laserGun.IsShooting());
            Tuple<string, float> suggestedCommandFromDistance = Grammar.GetSuggestedCommandFromDistance(phrase);

            // If confidence is greater than 0 for fromdata use that command
            if (suggestedCommandFromData.Item2 > 0) return new Tuple<string, float, bool, string>(suggestedCommandFromData.Item1, suggestedCommandFromData.Item2, true, phrase);
            
            // If confidence is greater than 0.5 for fromdistance ask the user
            if (suggestedCommandFromDistance.Item2 > 0.5) return new Tuple<string, float, bool, string>(suggestedCommandFromDistance.Item1, suggestedCommandFromDistance.Item2, false, phrase);
            
            return null; // Otherwise no command was found
        }

        private static void ReadTextToSpeech(string phrase) {
            Application.ExternalCall("readTextToSpeech", phrase);
        }

        public static void StartSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("startVoiceRecognition");
        }

        public static void StopSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("stopVoiceRecognition");
        }
    }
}
