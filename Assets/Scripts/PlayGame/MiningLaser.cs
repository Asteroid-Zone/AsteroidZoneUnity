using UnityEngine;

namespace PlayGame {
    public class MiningLaser : MonoBehaviour {

        public LineRenderer laser;

        private const int MiningRange = 50;
        private const int MiningSpeed = 20; // fastest speed = 0

        private int _lastFrameMined = 0;

        private void Start() {
            laser.positionCount = 2;
            laser.enabled = false;
        }
        
        private void Update() {
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit);
            
            if (hit.collider) {
                UpdateLaser((int) hit.distance);
                if (hit.collider.gameObject.CompareTag("Asteroid")) {
                    MineAsteroid(hit.collider.gameObject);
                }
            } else {
                UpdateLaser(MiningRange);
            }
        }

        private void MineAsteroid(GameObject asteroid) {
            if (Time.frameCount - _lastFrameMined > MiningSpeed) {
                asteroid.GetComponent<Asteroid>().MineAsteroid();
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