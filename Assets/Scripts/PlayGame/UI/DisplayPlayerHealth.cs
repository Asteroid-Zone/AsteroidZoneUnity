using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class DisplayPlayerHealth : MonoBehaviour {
    
        public GameObject player;

        private PlayerData _playerData;
        private Text _text;

        private void Start() {
            player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _text = GetComponent<Text>();
            _playerData = player.GetComponent<PlayerData>();
        }

        private void Update() {
            _text.text = "Health: " + _playerData.GetHealth() + "/" + _playerData.GetMaxHealth();
        }
    }
}
