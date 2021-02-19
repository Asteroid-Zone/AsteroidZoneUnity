using UnityEngine;

namespace PlayGame {
    public class LaserGun : MonoBehaviour {

        public GameObject laserPrefab;
        private PlayerData _playerData;

        private int _lastFrameFired = 0;
        private const int ShotDelay = 20; // Number of frames to wait between shooting

        private void Start() {
            _playerData = GetComponent<PlayerData>();
        }

        public void Shoot() {
            if (Time.frameCount - _lastFrameFired > ShotDelay) { // Only fire every x frames
                Vector3 spawnPosition = transform.position + (transform.forward * 2); // Offset the laser so it doesn't spawn in the players ship
                GameObject laser = Instantiate(laserPrefab, spawnPosition, transform.rotation);
                laser.transform.Rotate(new Vector3(90, 0, 0)); // Rotate the laser so its not facing up
                laser.GetComponent<Rigidbody>().AddForce(transform.forward * _playerData.GetLaserSpeed());
                _lastFrameFired = Time.frameCount;
            }
        }
    }
}
