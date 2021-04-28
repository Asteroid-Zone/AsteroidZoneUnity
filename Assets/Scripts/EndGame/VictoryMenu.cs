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
    
    /// <summary>
    /// This class controls the Victory Scene.
    /// </summary>
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

            _playerStats = GetPlayerStats();
            
            playerName.text += _playerStats.playerName;
            playerResourcesHarvested.text += _playerStats.resourcesHarvested;
            playerAsteroidsDestroyed.text += _playerStats.asteroidsDestroyed;
            playerPiratesDestroyed.text += _playerStats.piratesDestroyed;

            gameTime.text += FormatTime(StatsManager.GameStats.gameTime);
            gameResourcesHarvested.text += StatsManager.GameStats.resourcesHarvested;
            gameAsteroidsDestroyed.text += StatsManager.GameStats.asteroidsDestroyed;
            gamePiratesDestroyed.text += StatsManager.GameStats.piratesDestroyed;
        }

        /// <summary>
        /// Finds the local players stats and destroys the players <c>GameObject</c>.
        /// </summary>
        /// <returns>
        /// <c>PlayerStats</c> containing the correct players stats.
        /// </returns>
        /// <exception cref="Exception">Thrown if no <c>PlayerStats</c> were found for the local player.</exception>
        private PlayerStats GetPlayerStats() {
            if (!DebugSettings.Debug) {
                foreach (GameObject player in PlayerData.Players) {
                    if (player != null && player.GetPhotonView().IsMine) {
                        PlayerStats pStats = StatsManager.GetPlayerStats(player.GetPhotonView().ViewID);
                        Destroy(player);
                        return pStats;
                    }
                }
            } else {
                return StatsManager.PlayerStatsList[0];
            }

            throw new Exception("Error - Player Stats Could Not Be Found");
        }

        /// <summary>
        /// Converts an amount of time to a formatted string (minutes:seconds:milliseconds).
        /// </summary>
        /// <param name="seconds">Time in seconds (<c>float</c>).</param>
        /// <returns>A formatted string representing the amount of time.</returns>
        private static string FormatTime(float seconds) {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            return ts.ToString(@"mm\:ss\:fff");
        }
        
        /// <summary>
        /// <para>Method is called when the player presses the 'Back to Menu' button.</para>
        /// Plays the button press sound effect then leaves the <c>PhotonRoom</c>.
        /// </summary>
        public void BackToMenu() {
            buttonPress.Play();
            PhotonNetwork.LeaveRoom();
        }
        
        /// <summary>
        /// <para>Method is called when the local player has finished leaving the room.</para>
        /// Resets the game and load the menu.
        /// </summary>
        public override void OnLeftRoom() {
            CleanUpGameObjects();
            GameManager.ResetStaticVariables();
            SceneManager.LoadScene(Scenes.MainMenuScene);
            
            base.OnLeftRoom();
        }

        /// <summary>
        /// Destroys all remaining game objects except PhotonMono.
        /// </summary>
        private void CleanUpGameObjects() {
            foreach (GameObject g in FindObjectsOfType<GameObject>()) {
                if (!g.name.Equals("PhotonMono")) Destroy(g);
            }
        }
    }
}
