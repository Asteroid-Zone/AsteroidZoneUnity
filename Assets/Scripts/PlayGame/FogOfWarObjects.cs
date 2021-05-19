using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGame {
    
    /// <summary>
    /// This class controls the fog of war.
    /// </summary>
    public class FogOfWarObjects : MonoBehaviour {

        private GameObject _player;
        private PlayerData _playerData;

        public GameObject asteroidSpawner;
        public GameObject pirateSpawner;

        private void Start() {
            if (!DebugSettings.FogOfWar) return;
            
            _player = (!DebugSettings.Debug && SceneManager.GetActiveScene().name != Scenes.TutorialScene) ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _playerData = _player.GetComponent<PlayerData>();
        }

        private void OnPreCull() {
            if (!DebugSettings.FogOfWar) return;
            if(_playerData.GetRole() == Role.StationCommander) return;
            
            RadiusFOW();
        }

        /// <summary>
        /// Show/hide each GameObject that is included in fog of war depending on its distance from the player.
        /// </summary>
        private void RadiusFOW() {
            // Asteroids
            foreach (Transform asteroid in asteroidSpawner.transform) {
                if (Vector3.Distance(_player.transform.position, asteroid.position) < _playerData.GetLookRadius()) {
                    asteroid.gameObject.layer = 0;
                } else {
                    asteroid.gameObject.layer = 31;
                }
            }
            
            // Pirates
            foreach (Transform pirate in pirateSpawner.transform) {
                if (Vector3.Distance(_player.transform.position, pirate.position) < _playerData.GetLookRadius()) {
                    pirate.gameObject.layer = 0;
                    pirate.GetChild(1).gameObject.layer = 0;
                } else {
                    pirate.gameObject.layer = 31;
                    pirate.GetChild(1).gameObject.layer = 31;
                }
            }

            // Players
            foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag(Tags.PlayerModelTag)) {
                if (Vector3.Distance(_player.transform.position, playerObject.transform.position) < _playerData.GetLookRadius()) {
                    if (playerObject.layer != 0) SetLayerRecursively(playerObject, 0);
                } else {
                    if (playerObject.layer != 31) SetLayerRecursively(playerObject, 31);
                }
            }
        }
        
        /// <summary>
        /// Recursively sets the layer of a GameObject and its child Transforms.
        /// </summary>
        /// <param name="o">Parent GameObject.</param>
        /// <param name="newLayer">Layer to set each object to.</param>
        private void SetLayerRecursively(GameObject o, int newLayer) {
            o.layer = newLayer;

            foreach (Transform child in o.transform){
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

    }
}
