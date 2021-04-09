using Photon.Pun;
using PlayGame.Pirates;
using Statics;
using UnityEngine;

namespace PlayGame.Player {
  
    public class PlayerLaserProjectile : MonoBehaviour {
        
        private Vector3 _startPosition;
        private PlayerData _shootingPlayerData;

        private void Start() {
            _startPosition = transform.position;
        }

        private void Update() {
            transform.Translate(-transform.forward * (_shootingPlayerData.GetLaserSpeed() * Time.deltaTime));
            if (Vector3.Distance(transform.position, _startPosition) > _shootingPlayerData.GetLaserRange()) {
                Destroy(gameObject); // Limit the lasers range
            }
        }

        private void OnTriggerEnter(Collider collision) {
            try {
                // todo play animation (explosion)

                if (collision.gameObject.CompareTag(Tags.PirateTag)) {
                    if ((!DebugSettings.Debug && PhotonNetwork.IsMasterClient) || DebugSettings.Debug) {
                        PirateData pirateData = collision.gameObject.GetComponent<PirateData>();
                        pirateData.TakeDamage(_shootingPlayerData);
                    }
                }
                
                if (collision.gameObject.CompareTag(Tags.AsteroidTag)) {
                    Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
                    asteroid.MineAsteroid(GameConstants.PlayerLaserMiningRate, _shootingPlayerData);
                }
            }
            finally
            {
                // Always destroy the laser in the end
                Destroy(gameObject);
            }
        }

        public void SetShootingPlayerData(PlayerData shootingPlayerData) {
            _shootingPlayerData = shootingPlayerData;
        }
    }
}
