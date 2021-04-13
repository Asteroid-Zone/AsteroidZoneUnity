using System.Collections.Generic;
using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;

namespace PlayGame {
    public class FogOfWarObjects : MonoBehaviour {

        private GameObject _player;
        private PlayerData _playerData;
        
        // Tile FOW Objects
        private GameObject[] _asteroids;
        private GameObject[] _pirates;
        public GameObject gridManagerObject;
        private FogOfWarTiles _fogOfWarTiles;

        // Radius FOW Objects
        public GameObject asteroidSpawner;
        public GameObject pirateSpawner;

        private void Start() {
            if (!DebugSettings.FogOfWar) return;
            _player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _playerData = _player.GetComponent<PlayerData>();
            if (_playerData.GetRole() == Role.StationCommander) return;
            
            _fogOfWarTiles = gridManagerObject.GetComponent<FogOfWarTiles>();
        }

        private void OnPreCull() {
            if (!DebugSettings.FogOfWar) return;
            if(_playerData.GetRole() == Role.StationCommander) return;
            
            if (DebugSettings.TileBasedFogOfWar) TileFOW();
            else RadiusFOW();
        }

        private void RadiusFOW() {
            foreach (Transform asteroid in asteroidSpawner.transform) {
                if (Vector3.Distance(_player.transform.position, asteroid.transform.position) < _playerData.GetLookRadius()) {
                    asteroid.gameObject.layer = 0;
                } else {
                    asteroid.gameObject.layer = 31;
                }
            }
            
            foreach (Transform pirate in pirateSpawner.transform) {
                if (Vector3.Distance(_player.transform.position, pirate.transform.position) < _playerData.GetLookRadius()) {
                    pirate.gameObject.layer = 0;
                    pirate.GetChild(1).gameObject.layer = 0;
                } else {
                    pirate.gameObject.layer = 31;
                    pirate.GetChild(1).gameObject.layer = 31;
                }
            }
        }

        private void TileFOW() {
            // TODO: Improve performance by having objects register themselves
            _asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
            _pirates = GameObject.FindGameObjectsWithTag("Pirate");
            List<Vector2> visibleTiles = _fogOfWarTiles.GetVisibleTiles();
            foreach (GameObject asteroid in _asteroids) {
                Vector2 position = GridManager.GlobalToGridCoord(asteroid.transform.position);
                if (visibleTiles.Contains(position))
                {
                    asteroid.layer = 0;
                }
                else
                {
                    asteroid.layer = 31;
                }
            }
            foreach (GameObject pirate in _pirates)
            {
                Vector2 position = GridManager.GlobalToGridCoord(pirate.transform.position);
                if (visibleTiles.Contains(position))
                {
                    pirate.layer = 0;
                    pirate.transform.GetChild(1).gameObject.layer = 0;
                }
                else
                {
                    pirate.layer = 31;
                    pirate.transform.GetChild(1).gameObject.layer = 31;
                }
            }
        }

    }
}
