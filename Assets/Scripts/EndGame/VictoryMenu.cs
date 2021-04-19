using System;
using Photon.Pun;
using PlayGame;
using PlayGame.Player;
using PlayGame.Stats;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EndGame {
    public class VictoryMenu : MonoBehaviourPunCallbacks {
    
        public AudioSource buttonPress;

        public Text winText;

        public Text playerName;
        public Text playerResourcesHarvested;
        public Text playerAsteroidsDestroyed;
        public Text playerPiratesDestroyed;

        public Text gameTime;
        public Text gameResourcesHarvested;
        public Text gameAsteroidsDestroyed;
        public Text gamePiratesDestroyed;
        
        private PlayerStats _playerStats;
        
        private void Start() {
            winText.text = StatsManager.GameStats.victory ? "You Win!" : "You lose!";

            if (!DebugSettings.Debug) {
                foreach (GameObject player in PlayerData.Players) {
                    if (player != null && player.GetPhotonView().IsMine) {
                        _playerStats = StatsManager.GetPlayerStats(player.GetPhotonView().ViewID);
                        Destroy(player);
                    }
                }
            } else _playerStats = StatsManager.PlayerStatsList[0];
            
            playerName.text += _playerStats.playerName;
            playerResourcesHarvested.text += _playerStats.resourcesHarvested;
            playerAsteroidsDestroyed.text += _playerStats.asteroidsDestroyed;
            playerPiratesDestroyed.text += _playerStats.piratesDestroyed;

            gameTime.text += FormatTime(StatsManager.GameStats.gameTime);
            gameResourcesHarvested.text += StatsManager.GameStats.resourcesHarvested;
            gameAsteroidsDestroyed.text += StatsManager.GameStats.asteroidsDestroyed;
            gamePiratesDestroyed.text += StatsManager.GameStats.piratesDestroyed;
        }

        private static string FormatTime(float seconds) {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            return ts.ToString(@"mm\:ss\:fff");
        }
        
        public void BackToMenu() {
            buttonPress.Play();
            PhotonNetwork.LeaveRoom();
        }
        
        /// Called when the local player left the room
        /// Delete all remaining game objects and load the menu
        public override void OnLeftRoom() {
            CleanUpGameObjects();
            GameManager.ResetStaticVariables();
            SceneManager.LoadScene(Scenes.MainMenuScene);
            
            base.OnLeftRoom();
        }

        private void CleanUpGameObjects() {
            foreach (GameObject g in FindObjectsOfType<GameObject>()) {
                if (!g.name.Equals("PhotonMono")) Destroy(g);
            }
        }
    }
}
