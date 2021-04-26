using PlayGame.UI;
using Statics;
using UnityEngine;
using System.Linq;

namespace PlayGame.Player {
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
            GameObject.FindGameObjectsWithTag(Tags.CombatLaserSfxTag).ToList().ForEach(miningSfx =>
            {
                if (miningSfx.transform.parent.parent == gameObject.transform)
                {
                    _laserSfx = miningSfx.GetComponent<AudioSource>();
                }
            });
            VolumeControl.AddSfxCSource(_laserSfx);
        }

        private void Update() {
            _timeSinceLastFired += (Time.deltaTime * 1000);
            if (_shooting && _timeSinceLastFired > _shotDelay) Shoot(); // Only fire every x ms
        }

        private void Shoot() {
            GameObject laser = Instantiate(laserPrefab, spawnTransform.position, transform.rotation);
            laser.GetComponent<PlayerLaserProjectile>().SetShootingPlayerData(_playerData); // Provide a reference to the player who shot the laser to the projectile
            laser.transform.Rotate(new Vector3(-90, 0, 0)); // Rotate the laser so its not facing up
            _timeSinceLastFired = 0;
            _laserSfx.Play();
        }

        public void StartShooting() {
            _shooting = true;
        }
        
        public void StopShooting() {
            _shooting = false;
        }

        public bool IsShooting() {
            return _shooting;
        }

        public void ReduceShotDelay(int amount) {
            _shotDelay -= amount;
        }
    }
}
