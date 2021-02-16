using UnityEngine;

namespace PlayGame {
    public class MiningLaser : MonoBehaviour {

        public LineRenderer laser;

        private const int MAXLength = 50;

        private void Start() {
            laser.positionCount = 2;
            laser.enabled = false;
        }
        
        private void Update() {
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit);

            if (hit.collider) {
                UpdateLaser((int) hit.distance);
            } else {
                UpdateLaser(MAXLength);
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