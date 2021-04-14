using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;

namespace PlayGame {
    public class FogOfWarObjects : MonoBehaviour {

        private GameObject _player;
        private PlayerData _playerData;

        // Radius FOW Objects
        public GameObject asteroidSpawner;
        public GameObject pirateSpawner;

        private void Start() {
            if (!DebugSettings.FogOfWar) return;
            _player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _playerData = _player.GetComponent<PlayerData>();
        }

        private void OnPreCull() {
            if (!DebugSettings.FogOfWar) return;
            if(_playerData.GetRole() == Role.StationCommander) return;
            
            RadiusFOW();
        }

        private void RadiusFOW() {
            foreach (Transform asteroid in asteroidSpawner.transform) {
                if (Vector3.Distance(_player.transform.position, asteroid.position) < _playerData.GetLookRadius()) {
                    asteroid.gameObject.layer = 0;
                } else {
                    asteroid.gameObject.layer = 31;
                }
            }
            
            foreach (Transform pirate in pirateSpawner.transform) {
                if (Vector3.Distance(_player.transform.position, pirate.position) < _playerData.GetLookRadius()) {
                    pirate.gameObject.layer = 0;
                    pirate.GetChild(1).gameObject.layer = 0;
                } else {
                    pirate.gameObject.layer = 31;
                    pirate.GetChild(1).gameObject.layer = 31;
                }
            }

            foreach (GameObject player in PlayerData.Players) {
                if (Vector3.Distance(_player.transform.position, player.transform.position) < _playerData.GetLookRadius()) {
                    player.gameObject.layer = 0;
                } else {
                    player.gameObject.layer = 31;
                }
            }
        }

    }
}
