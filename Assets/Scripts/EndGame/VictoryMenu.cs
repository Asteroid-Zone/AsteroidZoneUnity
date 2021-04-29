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

        //public Text gameTime;

        public GameObject scoreboard;
        public GameObject playerScorePanel;

        private void Start() {
            winText.text = StatsManager.GameStats.victory ? "You Win!" : "You lose!";

            foreach (PlayerStats stats in StatsManager.PlayerStatsList) {
                GameObject playerPanel = Instantiate(playerScorePanel, scoreboard.transform);
                Text[] texts = playerPanel.GetComponentsInChildren<Text>();
                texts[0].text = stats.playerName;
                texts[1].text = StatsManager.GetEndRole(stats.photonID).ToString();
                texts[2].text = stats.resourcesHarvested.ToString();
                texts[3].text = stats.piratesDestroyed.ToString();
                texts[4].text = StatsManager.GetPlayerScore(stats.photonID).ToString();
            }

            //gameTime.text += FormatTime(StatsManager.GameStats.gameTime);
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
