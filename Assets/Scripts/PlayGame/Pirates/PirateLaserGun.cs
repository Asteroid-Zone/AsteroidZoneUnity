using UnityEngine;

namespace PlayGame.Pirates {
    public class PirateLaserGun : MonoBehaviour {

        public GameObject laserPrefab;
        private PirateData _pirateData;

        private int _lastFrameFired;
        private const int ShotDelay = 100; // Number of frames to wait between shooting

        private bool _shooting;

        private void Start() {
            _pirateData = GetComponent<PirateData>();
        }

        private void Update() {
            if (_shooting && Time.frameCount - _lastFrameFired > ShotDelay) Shoot(); // Only fire every x frames
        }

        private void Shoot() {
            Vector3 spawnPosition = transform.position + (transform.forward * 2); // Offset the laser so it doesn't spawn in the players ship
            GameObject laser = Instantiate(laserPrefab, spawnPosition, transform.rotation);
            laser.GetComponent<PirateLaserProjectile>().SetShootingPirateData(_pirateData); // Provide a reference to the pirate who shot the laser to the projectile
            laser.transform.Rotate(new Vector3(90, 0, 0)); // Rotate the laser so its not facing up
            laser.GetComponent<Rigidbody>().AddForce(transform.forward * PirateData.LaserSpeed);
            _lastFrameFired = Time.frameCount;
        }

        public void StartShooting() {
            _shooting = true;
        }
        
        public void StopShooting() {
            _shooting = false;
        }
    }
}
