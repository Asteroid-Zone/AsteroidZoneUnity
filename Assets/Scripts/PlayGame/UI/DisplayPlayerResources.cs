using PlayGame.Player;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI {
    public class DisplayPlayerResources : MonoBehaviour {
    
        public GameObject player;

        private PlayerData _playerData;
        private Text _text;

        private void Start() {
            _text = GetComponent<Text>();
            _playerData = player.GetComponent<PlayerData>();
        }

        private void Update() {
            _text.text = "Resources: " + _playerData.GetResources();
        }
    }
}
