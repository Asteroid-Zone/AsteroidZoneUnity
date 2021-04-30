using Photon.Pun;
using PlayGame.Pirates;
using Statics;
using UnityEngine;

namespace PlayGame.Player {
    
    /// <summary>
    /// This class controls a players laser.
    /// </summary>
    public class PlayerLaserProjectile : MonoBehaviour {
        
        private Vector3 _startPosition;
        private PlayerData _shootingPlayerData;

        private void Start() {
            _startPosition = transform.position;
        }

        /// <summary>
        /// Moves the laser.
        /// <para>Destroys the laser if it has reached its range.</para>
        /// </summary>
        private void Update() {
            transform.Translate(-transform.forward * (_shootingPlayerData.GetLaserSpeed() * Time.deltaTime));
            if (Vector3.Distance(transform.position, _startPosition) > _shootingPlayerData.GetLaserRange()) {
                Destroy(gameObject); // Limit the lasers range
            }
        }

        /// <summary>
        /// <para>Method is called when the laser hits a GameObject.</para>
        /// Damages the object it collided with then destroys the laser.
        /// <remarks>This method only damages a pirate if called by the host or in debug mode.</remarks>
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter(Collider collision) {
            try {
                // todo play animation (explosion)

                if (collision.gameObject.CompareTag(Tags.PirateTag)) {
                    _shootingPlayerData.IncreaseCombatXP(Random.Range(GameConstants.MinXPCombatHit, GameConstants.MaxXPCombatHit));
                    
                    if ((!DebugSettings.Debug && PhotonNetwork.IsMasterClient) || DebugSettings.Debug) {
                        PirateData pirateData = collision.gameObject.GetComponent<PirateData>();
                        pirateData.TakeDamage(_shootingPlayerData);
                    }
                }
                
                if (collision.gameObject.CompareTag(Tags.AsteroidTag)) {
                    Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
                    asteroid.MineAsteroid(GameConstants.PlayerLaserMiningRate, _shootingPlayerData);
                }
            } finally {
                // Always destroy the laser in the end
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Stores the PlayerData that fired the laser.
        /// </summary>
        /// <param name="shootingPlayerData"></param>
        public void SetShootingPlayerData(PlayerData shootingPlayerData) {
            _shootingPlayerData = shootingPlayerData;
        }
    }
}
