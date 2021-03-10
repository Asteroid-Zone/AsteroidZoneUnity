using Photon.Pun;
using PlayGame.Stats;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EndGame {
    public class VictoryMenu : MonoBehaviour {
    
        public AudioSource buttonPress;

        public Text playerName;
        public Text playerResourcesHarvested;
        public Text playerAsteroidsDestroyed;
        public Text playerPiratesDestroyed;

        public Text gameResourcesHarvested;
        public Text gameAsteroidsDestroyed;
        public Text gamePiratesDestroyed;
        
        private PlayerStats _playerStats;
        
        private void Start() {
            _playerStats = StatsManager.GetPlayerStats(PhotonNetwork.NickName);
            playerName.text += _playerStats.playerName;
            playerResourcesHarvested.text += _playerStats.resourcesHarvested;
            playerAsteroidsDestroyed.text += _playerStats.asteroidsDestroyed;
            playerPiratesDestroyed.text += _playerStats.piratesDestroyed;
            
            gameResourcesHarvested.text += StatsManager.GameStats.resourcesHarvested;
            gameAsteroidsDestroyed.text += StatsManager.GameStats.asteroidsDestroyed;
            gamePiratesDestroyed.text += StatsManager.GameStats.piratesDestroyed;
        }

        public void BackToMenu() {
            buttonPress.Play();
            SceneManager.LoadScene(Scenes.MainMenuScene);
        }
    }
}
