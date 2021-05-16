using System.Collections.Generic;
using Photon;
using Photon.GameControllers;
using Photon.Pun;
using PlayGame.Pirates;
using PlayGame.Player;
using PlayGame.Stats;
using PlayGame.UI;
using PlayGame.VoiceChat;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGame {
    public class GameManager : MonoBehaviourPunCallbacks {

        public enum GameOverType {
            Victory,
            StationDestroyed,
            TimeUp
        }

        public static bool gameOver = false;

        public static void ResetStaticVariables() {
            gameOver = false;
            PirateController.ResetStaticVariables();
            PlayerData.Players = new List<GameObject>();
            StatsManager.PlayerStatsList.Clear();
            StatsManager.GameStats.Reset();
            
            GameSetup.Instance = null;
            PhotonPlayer.Instance = null;
            PhotonLobby.ResetStaticVariables();
            PhotonRoom.ResetStaticVariables();
            
            PirateSpawner.ResetStaticVariables();
            TestPlayer.ResetStaticVariables();
            AsteroidSpawner.ResetStaticVariables();
            EventsManager.ResetStaticVariables();
        }

        private void Update() {
            if (Time.time - StatsManager.GameStats.startTime > GameConstants.TimeLimit) GameOver(GameOverType.TimeUp);
        }

        public void GameOver(GameOverType gameOverType) {
            if (gameOver) return; // Ensures LeaveRoom is only called once
            if (SceneManager.GetActiveScene().name == Scenes.TutorialScene) return; // Game can't end if player is in the tutorial

            // Unmute everyone for the cutscene and stats screen
            VoiceChatController.UnmuteMyselfInVoiceChat();
            
            if (!DebugSettings.Debug && PhotonNetwork.IsMasterClient) photonView.RPC(nameof(RPC_GameOver), RpcTarget.AllBuffered, gameOverType, Time.time - StatsManager.GameStats.startTime);
            else if (DebugSettings.Debug) RPC_GameOver(gameOverType, Time.time - StatsManager.GameStats.startTime);
        }
        
        [PunRPC]
        public void RPC_GameOver(GameOverType gameOverType, float time) {
            gameOver = true;
            string eventMessage = "Game Over";
            if (gameOverType == GameOverType.Victory) eventMessage = "Game completed";
            EventsManager.AddMessage(eventMessage);
            StatsManager.GameStats.victory = gameOverType == GameOverType.Victory;
            StatsManager.GameStats.gameOverType = gameOverType;
            StatsManager.GameStats.gameTime = time;

            // Set final levels
            foreach (GameObject player in PlayerData.Players) {
                player.GetComponent<PlayerData>().SetFinalLevels();
            }
            
            SceneManager.LoadScene(Scenes.EndCutsceneScene);
        }

        /// Called when the local player left the room. We need to load the launcher scene.
        public override void OnLeftRoom() {
            CleanUpGameObjects();
            ResetStaticVariables();
            SceneManager.LoadScene(Scenes.MainMenuScene);
            
            base.OnLeftRoom();
        }
        
        private void CleanUpGameObjects() {
            foreach (GameObject g in FindObjectsOfType<GameObject>()) {
                if (!g.name.Equals("PhotonMono")) Destroy(g);
            }
        }

        public void LeaveRoom() {
            if (SceneManager.GetActiveScene().name == Scenes.TutorialScene)
            {
                SceneManager.LoadScene(Scenes.MainMenuScene);
            }
            else
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        private void LoadArena() {
            if (!PhotonNetwork.IsMasterClient) {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Play Game");
            PhotonNetwork.LoadLevel(Scenes.PlayGameScene);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
        {
            PlayerData.UpdatePlayerLists();
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        }


    }
}
