using Statics;
using UnityEngine;

namespace PlayGame.Player {
    public class MiningLaser : MonoBehaviour {

        private PlayerData _playerData;
        private AudioSource _miningLaserSfx;
        
        public LineRenderer laser;

        private static int MiningRange = 20;
        private const int MiningRate = 8; // Amount of resources gathered every mining tick
        private const int MiningDelay = 20; // Number of frames to wait between mining

        private int _lastFrameMined = 0;

        private void Start() {
            _playerData = GetComponent<PlayerData>();
            laser.positionCount = 2;
            laser.enabled = false;
            
            // 2nd child is SFX, 1st child is mining laser SFX
            _miningLaserSfx = gameObject.transform.GetChild(2).GetChild(1).GetComponent<AudioSource>();
            _miningLaserSfx.loop = true;
            if (DebugSettings.InfiniteMiningRange) MiningRange = 10000;
        }
        
        private void Update() {
            if (laser.enabled)
            {
                if (!_miningLaserSfx.isPlaying)
                {
                    _miningLaserSfx.Play();
                }
                RaycastHit hit;
                Physics.Raycast(transform.position, transform.forward, out hit, MiningRange); // Get the game object that the laser is hitting

                if (hit.collider) { // If the laser is hitting a game object
                    UpdateLaser((int) hit.distance);
                    if (hit.collider.gameObject.CompareTag("Asteroid")) {
                        MineAsteroid(hit.collider.gameObject);
                    }
                } else {
                    UpdateLaser(MiningRange);
                }
            }
            else
            {
                _miningLaserSfx.Stop();
            }
        }

        private void MineAsteroid(GameObject asteroid) {
            if (Time.frameCount - _lastFrameMined > MiningDelay) { // Only mine the asteroid every x frames
                Asteroid asteroidScript = asteroid.GetComponent<Asteroid>();
                asteroidScript.MineAsteroid(MiningRate, _playerData);
                _playerData.AddResources(asteroidScript.GetResources(MiningRate));
                _lastFrameMined = Time.frameCount;
            }
        }
        
        private void UpdateLaser(int distance) {
            laser.SetPosition(1, new Vector3(0, 0, distance)); // Sets the end position of the laser
        }
        
        public void EnableMiningLaser() {
            laser.enabled = true;
        }

        public void DisableMiningLaser() {
            laser.enabled = false;
        }
    }
}