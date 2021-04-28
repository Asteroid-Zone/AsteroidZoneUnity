using UnityEngine;

namespace PlayGame.Pirates {
    
    /// <summary>
    /// This class controls the pirates laser gun.
    /// </summary>
    public class PirateLaserGun : MonoBehaviour {

        public GameObject laserPrefab;
        private PirateData _pirateData;

        private float _timeSinceLastFired = 0;

        private bool _shooting;

        private void Start() {
            _pirateData = GetComponent<PirateData>();
        }

        /// <summary>
        /// Shoots if the gun is shooting and it has been long enough since the last shot.
        /// </summary>
        private void Update() {
            _timeSinceLastFired += (Time.deltaTime * 1000);
            if (_shooting && _timeSinceLastFired > _pirateData.GetShotDelay()) Shoot(); // Only fire every x seconds
        }

        /// <summary>
        /// Instantiates a new PirateLaserProjectile.
        /// </summary>
        private void Shoot() {
            Vector3 spawnPosition = transform.position + (transform.forward * 2); // Offset the laser so it doesn't spawn in the pirate ship
            GameObject laser = Instantiate(laserPrefab, spawnPosition, transform.rotation);
            laser.GetComponent<PirateLaserProjectile>().SetShootingPirateData(_pirateData); // Provide a reference to the pirate who shot the laser to the projectile
            laser.transform.Rotate(new Vector3(-90, 0, 0)); // Rotate the laser so its not facing up
            _timeSinceLastFired = 0;
        }

        /// <summary>
        /// Starts shooting every x seconds.
        /// </summary>
        public void StartShooting() {
            _shooting = true;
        }
        
        /// <summary>
        /// Stops the laser gun from shooting.
        /// </summary>
        public void StopShooting() {
            _shooting = false;
        }
    }
}
