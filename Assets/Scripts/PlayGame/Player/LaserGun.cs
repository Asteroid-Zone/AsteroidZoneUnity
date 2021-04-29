using PlayGame.UI;
using Statics;
using UnityEngine;
using System.Linq;

namespace PlayGame.Player {
    
    /// <summary>
    /// This class controls the combat laser gun.
    /// </summary>
    public class LaserGun : MonoBehaviour {

        public GameObject laserPrefab;
        public Transform spawnTransform;
        private PlayerData _playerData;
        private AudioSource _laserSfx;
        
        private float _timeSinceLastFired = 0;

        private int _shotDelay;

        private bool _shooting;

        private void Start() {
            _playerData = GetComponent<PlayerData>();
            _shotDelay = GameConstants.PlayerShotDelay;

            // Get the combat laser SFX that has the necessary tag and is a child of the current player's game object
            // Note: it should be a child of the current player, because in multiplayer it wouldn't work otherwise
            GameObject.FindGameObjectsWithTag(Tags.CombatLaserSfxTag).ToList().ForEach(miningSfx => {
                if (miningSfx.transform.parent.parent == gameObject.transform) _laserSfx = miningSfx.GetComponent<AudioSource>();
            });
            VolumeControl.AddSfxCSource(_laserSfx);
        }

        /// <summary>
        /// Updates the time since last fired and shoot if it has been long enough.
        /// </summary>
        private void Update() {
            _timeSinceLastFired += (Time.deltaTime * 1000);
            if (_shooting && _timeSinceLastFired > _shotDelay) Shoot(); // Only fire every x ms
        }

        /// <summary>
        /// Instantiates a new laser and plays the sound effect.
        /// </summary>
        private void Shoot() {
            GameObject laser = Instantiate(laserPrefab, spawnTransform.position, transform.rotation);
            laser.GetComponent<PlayerLaserProjectile>().SetShootingPlayerData(_playerData); // Provide a reference to the player who shot the laser to the projectile
            laser.transform.Rotate(new Vector3(-90, 0, 0)); // Rotate the laser so its not facing up
            _timeSinceLastFired = 0;
            _laserSfx.Play();
        }

        /// <summary>
        /// Sets _shooting to true.
        /// </summary>
        public void StartShooting() {
            _shooting = true;
        }
        
        /// <summary>
        /// Sets _shooting to false.
        /// </summary>
        public void StopShooting() {
            _shooting = false;
        }

        /// <summary>
        /// Returns true if the laser is currently on.
        /// </summary>
        /// <returns></returns>
        public bool IsShooting() {
            return _shooting;
        }

        /// <summary>
        /// Reduces the delay between shots by the given amount.
        /// </summary>
        /// <param name="amount">The number of ms to reduce the shot delay by.</param>
        public void ReduceShotDelay(int amount) {
            _shotDelay -= amount;
        }
    }
}
