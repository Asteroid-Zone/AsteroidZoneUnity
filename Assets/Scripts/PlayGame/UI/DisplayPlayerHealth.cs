using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayGame.UI {
    
    /// <summary>
    /// This class displays the player's health in the parent text GameObject.
    /// </summary>
    public class DisplayPlayerHealth : MonoBehaviour {
    
        public GameObject player;

        private PlayerData _playerData;
        private Text _text;

        private void Start() {
            player = (!DebugSettings.Debug && SceneManager.GetActiveScene().name != Scenes.TutorialScene) ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _text = GetComponent<Text>();
            _playerData = player.GetComponent<PlayerData>();
        }

        private void Update() {
            _text.text = "Health: " + _playerData.GetHealth() + "/" + _playerData.GetMaxHealth();
        }
    }
}
