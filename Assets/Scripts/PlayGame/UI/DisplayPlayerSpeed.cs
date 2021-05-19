using Photon.GameControllers;
using PlayGame.Player;
using UnityEngine;
using UnityEngine.UI;
using Statics;
using UnityEngine.SceneManagement;

namespace PlayGame.UI {
    
    /// <summary>
    /// This class displays the player's current speed in the parent text GameObject.
    /// </summary>
    public class DisplayPlayerSpeed : MonoBehaviour {
    
        public GameObject player;

        private PlayerData _playerData;
        private Text _text;
        private Vector3 _lastPosition;

        private void Start() {
            player = (!DebugSettings.Debug && SceneManager.GetActiveScene().name != Scenes.TutorialScene) ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _text = GetComponent<Text>();
            _playerData = player.GetComponent<PlayerData>();
        }

        /// <summary>
        /// Calculates and displays the players current speed.
        /// </summary>
        private void LateUpdate() {
            if (player == null) return;
            
            Vector3 currentPosition = player.transform.position;
            float speed = (currentPosition - _lastPosition).magnitude / Time.deltaTime;
            _lastPosition = currentPosition;
            _text.text = $"Speed: {speed:0.00}/{_playerData.GetMaxSpeed()}";
        }
    }
}
