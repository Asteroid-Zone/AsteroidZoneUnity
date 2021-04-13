using Photon.GameControllers;
using PlayGame.Player;
using UnityEngine;
using UnityEngine.UI;
using Statics;

namespace PlayGame.UI
{
    public class DisplayPlayerSpeed : MonoBehaviour {
    
        public GameObject player;

        private PlayerData _playerData;
        private Text _text;
        private Vector3 _lastPosition;

        private void Start() {
            player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _text = GetComponent<Text>();
            _playerData = player.GetComponent<PlayerData>();
        }

        private void LateUpdate() {
            if (player == null) return;
            Vector3 currentPosition = player.transform.position;
            float speed = (currentPosition - _lastPosition).magnitude / Time.deltaTime;
            _lastPosition = currentPosition;
            _text.text = $"Speed: {speed:0.00}/{_playerData.GetMaxSpeed()}";
        }
    }
}
