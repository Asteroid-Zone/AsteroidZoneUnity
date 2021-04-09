using System.Collections.Generic;
using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;

namespace PlayGame {
    public class FogOfWarObjects : MonoBehaviour {

        private GameObject _player;
        
        // Tile FOW Objects
        private GameObject[] _asteroids;
        private GameObject[] _pirates;
        private GameObject _gridManagerObject;
        private GridManager _gridManager;
        private FogOfWarTiles _fogOfWarTiles;

        // Radius FOW Objects
        private GameObject _asteroidSpawner;
        private GameObject _pirateSpawner;

        void Start() {
            _player = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            
            _gridManagerObject = GameObject.Find("Grid Manager");
            _gridManager = _gridManagerObject.GetComponent<GridManager>();
            _fogOfWarTiles = _gridManagerObject.GetComponent<FogOfWarTiles>();
            
            _asteroidSpawner = GameObject.Find("Asteroid Spawner");
            _pirateSpawner = GameObject.Find("PirateSpawner");
        }

        private void OnPreCull() {
            if (DebugSettings.TileBasedFogOfWar) TileFOW();
            else RadiusFOW();
        }

        private void RadiusFOW() {
            foreach (Transform asteroid in _asteroidSpawner.transform) {
                if (Vector3.Distance(_player.transform.position, asteroid.transform.position) < GameConstants.PlayerLookRadius) {
                    asteroid.gameObject.layer = 0;
                } else {
                    asteroid.gameObject.layer = 31;
                }
            }
            
            foreach (Transform pirate in _pirateSpawner.transform) {
                if (Vector3.Distance(_player.transform.position, pirate.transform.position) < GameConstants.PlayerLookRadius) {
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
            List <Vector2> _visibleTiles = _fogOfWarTiles.GetVisibleTiles();
            foreach (GameObject asteroid in _asteroids) {
                Vector2 position = _gridManager.GlobalToGridCoord(asteroid.transform.position);
                if (_visibleTiles.Contains(position))
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
                Vector2 position = _gridManager.GlobalToGridCoord(pirate.transform.position);
                if (_visibleTiles.Contains(position))
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
