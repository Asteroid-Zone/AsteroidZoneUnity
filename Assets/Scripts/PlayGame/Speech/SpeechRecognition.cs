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

namespace PlayGame.Speech {
    public class SpeechRecognition : MonoBehaviourPun {

        public Text text;
        private string _myResponse = "...";

        public GameObject player;
        public GameObject spaceStation;
        public GameObject ping;

        private PlayerData _playerData;

        private ActionController _actionController;

        private void Start() {
            StartSpeechRecognitionInTheBrowser();
            player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();

            _playerData = player.GetComponent<PlayerData>();

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
            text.text = _myResponse;
        }

        private void OnDestroy() {
            StopSpeechRecognitionInTheBrowser();
        }

        // Called by javascript when speech is detected
        public void GetResponse(string result) {
            if (!DebugSettings.Debug && !PhotonPlayer.Instance.photonView.IsMine) return;

            _myResponse = result.ToLower();
            Command command = Grammar.GetCommand(_myResponse, _playerData, player.transform);

            if (command.IsValid()) {
                if (DebugSettings.Debug) _actionController.PerformActions(command);
                else photonView.RPC("RPC_PerformActions", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, _myResponse);
            } else {
                DisplaySuggestedCommand(_myResponse);
            }
        }

        // Displays the suggested command, if we are confident its correct perform the command
        private void DisplaySuggestedCommand(string phrase) {
            Tuple<string, float> suggestedCommandFromData = Grammar.GetSuggestedCommandFromData(phrase);
            Tuple<string, float> suggestedCommandFromDistance = Grammar.GetSuggestedCommandFromDistance(phrase);
            string eventMessage = "";

            if (suggestedCommandFromData.Item2 > 0) { // If confidence is greater than 0 for fromdata use that command
                if (suggestedCommandFromData.Item2 > 0.8) { // If confidence is greater than 0.8 try to perform the command
                    Command command = Grammar.GetCommand(suggestedCommandFromData.Item1, _playerData, player.transform);
                    
                    if (command.IsValid()) { // If command is valid perform it
                        if (DebugSettings.Debug) _actionController.PerformActions(command);
                        else photonView.RPC("RPC_PerformActions", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, _myResponse);
                        eventMessage = "'" + phrase + "' is an invalid command. We think you meant '" + suggestedCommandFromData + "' and have performed the action.";
                    } else { // If suggested command is invalid ask the user
                        eventMessage = "'" + phrase + "' is an invalid command. Did you mean '" + suggestedCommandFromData + "' ? (confidence = " + suggestedCommandFromData.Item2 + ")";
                    }
                } else { // If confidence is less than 0.8 ask the user
                    eventMessage = "'" + phrase + "' is an invalid command. Did you mean '" + suggestedCommandFromData + "' ? (confidence = " + suggestedCommandFromData.Item2 + ")";
                }
            } else if (suggestedCommandFromDistance.Item2 > 0.5) { // If confidence is greater than 0.5 for fromdistance ask the user
                eventMessage = "'" + phrase + "' is an invalid command. Did you mean '" + suggestedCommandFromDistance + "' ? (confidence = " + suggestedCommandFromDistance.Item2 + ")";
            } else { // If we're not confident in either command tell the user we didnt understand
                eventMessage = "'" + phrase + "' is an invalid command. We're not sure what you meant.";
            }
            
            EventsManager.AddMessage(eventMessage);
            ReadTextToSpeech(eventMessage); // Read the message using tts in the browser
        }

        [PunRPC]
        public void RPC_PerformActions(int viewID, string phrase) {
            if (player == null) return;
            GameObject prevPlayer = player;
            List<GameObject> playerList = player.GetComponent<PlayerData>().GetList();
            foreach (GameObject p in playerList) {
                if (p != null && viewID == p.GetComponent<PhotonView>().ViewID) player = p;
            }
            
            Command command = Grammar.GetCommand(phrase, player.GetComponent<PlayerData>(), player.transform);

            _actionController = CreateActionController(player);
            _actionController.PerformActions(command);
            player = prevPlayer;
        }
        
        private static void ReadTextToSpeech(string phrase) {
            Application.ExternalCall("readTextToSpeech", phrase);
        }

        private static void StartSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("startVoiceRecognition");
        }

        private static void StopSpeechRecognitionInTheBrowser() {
            Application.ExternalCall("stopVoiceRecognition");
        }

    }
}
