using UnityEngine;

namespace PlayGame.Player {
    public class MiningLaser : MonoBehaviour {

        private PlayerData _playerData;
        
        public LineRenderer laser;

        private const int MiningRange = 20;
        private const int MiningDelay = 20; // Number of frames to wait between mining

        private int _lastFrameMined = 0;

        private void Start() {
            _playerData = GetComponent<PlayerData>();
            laser.positionCount = 2;
            laser.enabled = false;
        }
        
        private void Update() {
            if (laser.enabled) {
                RaycastHit hit;
                Physics.Raycast(transform.position, transform.forward, out hit); // Get the game object that the laser is hitting

                if (hit.collider) { // If the laser is hitting a game object
                    UpdateLaser((int) hit.distance);
                    if (hit.collider.gameObject.CompareTag("Asteroid")) {
                        MineAsteroid(hit.collider.gameObject);
                    }
                } else {
                    UpdateLaser(MiningRange);
                }
            }
        }

        private void MineAsteroid(GameObject asteroid) {
            if (Time.frameCount - _lastFrameMined > MiningDelay) { // Only mine the asteroid every x frames
                Asteroid asteroidScript = asteroid.GetComponent<Asteroid>();
                asteroidScript.MineAsteroid();
                _playerData.AddResources(asteroidScript.GetResources());
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