using Photon.Pun;
using PlayGame.Stats;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EndGame {
    public class VictoryMenu : MonoBehaviour {
    
        public AudioSource buttonPress;

        public Text name;
        public Text resourcesHarvested;

        private PlayerStats _playerStats;
        
        private void Start() {
            _playerStats = StatsManager.GetPlayerStats(PhotonNetwork.NickName);
            name.text += _playerStats.playerName;
            resourcesHarvested.text += _playerStats.resourcesHarvested;
        }

        public void BackToMenu() {
            buttonPress.Play();
            SceneManager.LoadScene(Scenes.MainMenuScene);
        }
    }
}
