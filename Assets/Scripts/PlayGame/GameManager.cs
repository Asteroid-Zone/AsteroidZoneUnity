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
    
    /// <summary>
    /// This class controls the game ending and people leaving/joining.
    /// </summary>
    public class GameManager : MonoBehaviourPunCallbacks {

        /// <summary>
        /// Type of game ending.
        /// </summary>
        public enum GameOverType {
            Victory,
            StationDestroyed,
            TimeUp
        }

        public static bool gameOver = false;

        /// <summary>
        /// Resets all the static variables in the game.
        /// </summary>
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
            // End the game if enough time has passed
            if (Time.time - StatsManager.GameStats.startTime > GameConstants.TimeLimit) GameOver(GameOverType.TimeUp);
        }

        /// <summary>
        /// This method ends the game.
        /// <remarks>This method can only be called by the host or in debug mode.</remarks>
        /// </summary>
        /// <param name="gameOverType">Type of game ending.</param>
        public void GameOver(GameOverType gameOverType) {
            if (gameOver) return; // Ensures LeaveRoom is only called once
            if (SceneManager.GetActiveScene().name == Scenes.TutorialScene) return; // Game can't end if player is in the tutorial

            // Unmute everyone for the cutscene and stats screen
            VoiceChatController.UnmuteMyselfInVoiceChat();
            
            if (!DebugSettings.Debug && PhotonNetwork.IsMasterClient) photonView.RPC(nameof(RPC_GameOver), RpcTarget.AllBuffered, gameOverType, Time.time - StatsManager.GameStats.startTime);
            else if (DebugSettings.Debug) RPC_GameOver(gameOverType, Time.time - StatsManager.GameStats.startTime);
        }
        
        /// <summary>
        /// Sets the end game stats and loads the cutscene.
        /// <remarks>This method is called via RPC.</remarks>
        /// </summary>
        /// <param name="gameOverType">Type of game ending.</param>
        /// <param name="time">Total game time.</param>
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

        /// <summary>
        /// Resets the game and loads the menu.
        /// Called when the local player left the room.
        /// </summary>
        public override void OnLeftRoom() {
            CleanUpGameObjects();
            ResetStaticVariables();
            SceneManager.LoadScene(Scenes.MainMenuScene);
            VoiceChatController.LeaveVoiceChat();
            
            base.OnLeftRoom();
        }
        
        /// <summary>
        /// Destroys any remaining GameObjects.
        /// Does not destroy PhotonMono as this causes issues when trying to start a new game.
        /// </summary>
        private void CleanUpGameObjects() {
            foreach (GameObject g in FindObjectsOfType<GameObject>()) {
                if (!g.name.Equals("PhotonMono")) Destroy(g);
            }
        }

        /// <summary>
        /// Leaves the PhotonRoom or loads the menu if the player is in the tutorial.
        /// </summary>
        public void LeaveRoom() {
            if (SceneManager.GetActiveScene().name == Scenes.TutorialScene) {
                StopAllCoroutines();
                DebugSettings.Debug = false;
                
                CleanUpGameObjects();
                ResetStaticVariables();
                SceneManager.LoadScene(Scenes.MainMenuScene);
            } else {
                PhotonNetwork.LeaveRoom();
            }
        }

        /// <summary>
        /// Loads the game scene.
        /// </summary>
        private void LoadArena() {
            if (!PhotonNetwork.IsMasterClient) {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Play Game");
            PhotonNetwork.LoadLevel(Scenes.PlayGameScene);
        }

        /// <summary>
        /// Logs in the console when a player joins the game.
        /// </summary>
        /// <param name="other">Player joining.</param>
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player other) {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        }

        /// <summary>
        /// Logs in the console when a player leaves the game.
        /// </summary>
        /// <param name="other">Player joining.</param>
        public override void OnPlayerLeftRoom(Photon.Realtime.Player other) {
            PlayerData.UpdatePlayerLists();
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        }

    }
}
