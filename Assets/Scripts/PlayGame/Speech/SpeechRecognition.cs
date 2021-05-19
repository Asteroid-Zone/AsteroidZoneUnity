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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayGame.Speech {
    
    /// <summary>
    /// This class controls the speech recognition.
    /// </summary>
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
        private Command _command = null; // Holds latest valid command
        private string _phrase = null; // Latest valid phrase
        private readonly List<string> _detectedPhrases = new List<string>();

        private void Start() {
            StartSpeechRecognitionInTheBrowser();
            player = (!DebugSettings.Debug && SceneManager.GetActiveScene().name != Scenes.TutorialScene) ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();

            _playerData = player.GetComponent<PlayerData>();
            _moveObject = player.GetComponent<MoveObject>();
            _miningLaser = player.GetComponent<MiningLaser>();
            _laserGun = player.GetComponent<LaserGun>();
            text = GameObject.FindGameObjectWithTag(Tags.SpeechTextTag).GetComponent<Text>();
            ping = GameObject.FindGameObjectWithTag(Tags.PingManagerTag);
            spaceStation = GameObject.FindGameObjectWithTag(Tags.StationTag);
            _actionController = CreateActionController(player);
        }

        /// <summary>
        /// This method creates an ActionController for the given player.
        /// </summary>
        /// <param name="playerObject"></param>
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
            if (!DebugSettings.Debug && !photonView.IsMine) return;
            text.text = _myResponse;
        }

        private void OnDestroy() {
            StopSpeechRecognitionInTheBrowser();
        }
        
        /// <summary>
        /// Creates an ActionController for the player with the given photonID and performs the action in the phrase.
        /// </summary>
        /// <param name="viewID"></param>
        /// <param name="phrase"></param>
        [PunRPC]
        public void RPC_PerformActions(int viewID, string phrase) {
            if (player == null) return;
            
            GameObject prevPlayer = player;

            player = PhotonView.Find(viewID).gameObject;

            Command command = Grammar.GetCommand(phrase, player.GetComponent<PlayerData>(), player.transform);

            _actionController = CreateActionController(player);
            _actionController.PerformActions(command);
            player = prevPlayer;
        }
        
        /// <summary>
        /// Resets the variables used in speech recognition.
        /// </summary>
        private void ResetSpeechRecognition() {
            _foundCommand = false;
            _command = null;
            _phrase = null;
            _detectedPhrases.Clear();
        }
        
        /// <summary>
        /// Parses the phrase to get a Command.
        /// If the command is valid, perform it.
        /// Method is called by javascript when speech is detected.
        /// </summary>
        /// <param name="result"></param>
        public void GetResponse(string result) {
            if (!DebugSettings.Debug && !photonView.IsMine) return;

            _myResponse = result.ToLower();
            
            _detectedPhrases.Add(_myResponse);
            Command command = Grammar.GetCommand(_myResponse, _playerData, player.transform);

            if (command.IsValid()) {
                _foundCommand = true;
                _command = command;
                _phrase = _myResponse;
                if (DebugSettings.Debug) _actionController.PerformActions(command);
                else photonView.RPC(nameof(RPC_PerformActions), RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, _myResponse);
            }
        }
        
        /// <summary>
        /// Parses the phrase to get a Command.
        /// If the command is valid, perform it.
        /// If no valid commands have been found since the last final speech, display the predicted command.
        /// Finally, resets the speech recognition.
        /// Method is called by javascript when final speech is detected.
        /// </summary>
        /// <param name="result"></param>
        public void GetFinalResponse(string result) {
            if (!DebugSettings.Debug && !photonView.IsMine) return;
            
            _myResponse = result.ToLower();
            
            _detectedPhrases.Add(_myResponse);
            Command command = Grammar.GetCommand(_myResponse, _playerData, player.transform);
            
            if (command.IsValid()) {
                _foundCommand = true;
                _command = command;
                _phrase = _myResponse;
            }

            if (_foundCommand) {
                if (DebugSettings.Debug) _actionController.PerformActions(_command);
                else photonView.RPC(nameof(RPC_PerformActions), RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, _phrase);
            } else {
                List<Tuple<string, float, bool, string>> suggestedCommands = FindSuggestedCommands(); // Tuple(command, confidence, fromData, phrase)
                Tuple<string, float, bool, string> suggestedCommand = FindBestSuggestedCommand(suggestedCommands);
                DisplaySuggestedCommand(suggestedCommand);
            }

            ResetSpeechRecognition();
        }

        /// <summary>
        /// Displays the suggested command and performs it if we are confident its what they meant.
        /// </summary>
        /// <param name="suggestedCommand"></param>
        private void DisplaySuggestedCommand(Tuple<string, float, bool, string> suggestedCommand) {
            string eventMessage;
            
            if (suggestedCommand == null) {
                eventMessage = "Invalid command. We're not sure what you meant.";
                EventsManager.AddMessage(eventMessage);
                ReadTextToSpeech(eventMessage);
                return;
            }
            
            string suggestedPhrase = suggestedCommand.Item1;
            string originalPhrase = suggestedCommand.Item4;
            float confidence = suggestedCommand.Item2;
            bool fromData = suggestedCommand.Item3;
            
            if (fromData) { // If command is from data
                if (confidence > 0.8) { // If confidence is greater than 0.8 try to perform the command
                    Command command = Grammar.GetCommand(suggestedPhrase, _playerData, player.transform);
                    
                    if (command.IsValid()) { // If command is valid perform it
                        if (DebugSettings.Debug) _actionController.PerformActions(command);
                        else photonView.RPC(nameof(RPC_PerformActions), RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, suggestedPhrase);
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

        /// <summary>
        /// Returns the most likely suggested command from a list of suggested commands.
        /// </summary>
        /// <param name="suggestedCommands"></param>
        /// <returns>Tuple(command, confidence, fromData, phrase)</returns>
        private Tuple<string, float, bool, string> FindBestSuggestedCommand(List<Tuple<string, float, bool, string>> suggestedCommands) {
            Tuple<string, float, bool, string> bestCommand = null;
            Tuple<string, float, bool, string> bestCommandFromData = null;
            Tuple<string, float, bool, string> bestCommandFromDistance = null;

            foreach (Tuple<string, float, bool, string> command in suggestedCommands) {
                if (command != null) {
                    if (command.Item1 == null) bestCommand = command;
                    else {
                        if (command.Item3) { // If command is from data
                            if (bestCommandFromData == null) bestCommandFromData = command;
                            else if (command.Item2 > bestCommandFromData.Item2) bestCommandFromData = command;
                        } else {
                            if (bestCommandFromDistance == null) bestCommandFromDistance = command;
                            else if (command.Item2 > bestCommandFromDistance.Item2) bestCommandFromDistance = command;
                        }
                    }
                }
            }

            if (bestCommandFromData != null && bestCommandFromData.Item2 > 0) bestCommand = bestCommandFromData;
            if (bestCommandFromDistance != null && bestCommandFromDistance.Item2 > 0.5) bestCommand = bestCommandFromDistance;
            
            return bestCommand;
        }
        
        /// <summary>
        /// Returns a list of suggested commands for all the detected phrases.
        /// </summary>
        /// <returns>Tuple(command, confidence, fromData, phrase)</returns>
        private List<Tuple<string, float, bool, string>> FindSuggestedCommands() {
            List<Tuple<string, float, bool, string>> suggestedCommands = new List<Tuple<string, float, bool, string>>();

            foreach (string phrase in _detectedPhrases) {
                suggestedCommands.Add(FindSuggestedCommand(phrase));
            }

            return suggestedCommands;
        }
        
        /// <summary>
        /// Returns the suggested command for a given phrase.
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns>Tuple(command, confidence, fromData, phrase)</returns>
        private Tuple<string, float, bool, string> FindSuggestedCommand(string phrase) {
            Tuple<string, float> suggestedCommandFromData;
            if (_playerData.GetRole() == Role.StationCommander) suggestedCommandFromData = Grammar.GetSuggestedCommandFromData(phrase, _playerData, null, false, false);
            else suggestedCommandFromData = Grammar.GetSuggestedCommandFromData(phrase, _playerData, _moveObject, _miningLaser.enabled, _laserGun.IsShooting());

            Tuple<string, float> suggestedCommandFromDistance = Grammar.GetSuggestedCommandFromDistance(phrase);

            // If confidence is greater than 0 for fromdata use that command
            if (suggestedCommandFromData.Item2 > 0) return new Tuple<string, float, bool, string>(suggestedCommandFromData.Item1, suggestedCommandFromData.Item2, true, phrase);
            
            // If confidence is greater than 0.5 for fromdistance ask the user
            if (suggestedCommandFromDistance.Item2 > 0.5) return new Tuple<string, float, bool, string>(suggestedCommandFromDistance.Item1, suggestedCommandFromDistance.Item2, false, phrase);
            
            return new Tuple<string, float, bool, string>(null, 0, false, phrase); // Otherwise no command was found
        }

        /// <summary>
        /// Calls readTextToSpeech() in the browser.
        /// </summary>
        /// <param name="phrase"></param>
        private static void ReadTextToSpeech(string phrase) {
            Application.ExternalCall("readTextToSpeech", phrase);
        }

        /// <summary>
        /// Calls startVoiceRecognition() in the browser.
        /// </summary>
        public static void StartSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("startVoiceRecognition");
        }

        /// <summary>
        /// Calls stopVoiceRecognition() in the browser.
        /// </summary>
        public static void StopSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("stopVoiceRecognition");
        }
    }
}
