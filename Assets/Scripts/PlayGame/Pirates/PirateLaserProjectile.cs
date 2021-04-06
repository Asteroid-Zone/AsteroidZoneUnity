using Photon.Pun;
using PlayGame.Player;
using Statics;
using UnityEngine;

namespace PlayGame.Pirates {

    public class PirateLaserProjectile : MonoBehaviour {
        
        private const int MiningRate = 4; // Amount of resources gathered every mining tick
        
        private Vector3 _startPosition;

        private PirateData _shootingPirateData;

        private void Start() {
            _startPosition = transform.position;
        }

        private void Update() {
            if (Vector3.Distance(transform.position, _startPosition) > _shootingPirateData.GetLaserRange()) Destroy(gameObject); // Limit the lasers range
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag(Tags.PirateTag)) return;
            if (collision.gameObject.CompareTag(Tags.CombatLaserTag)) return;
            
            // todo play animation (explosion)
            try
            {
                if (collision.gameObject.CompareTag(Tags.PlayerTag))
                {
                    PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
                    playerData.TakeDamage(_shootingPirateData.GetLaserDamage());
                }

                if (collision.gameObject.CompareTag(Tags.AsteroidTag))
                {
                    Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
                    asteroid.MineAsteroid(MiningRate, null);
                }

                if (collision.gameObject.CompareTag(Tags.StationTag))
                {
                    if ((!DebugSettings.Debug && PhotonNetwork.IsMasterClient) || DebugSettings.Debug)
                    {
                        SpaceStation.SpaceStation station =
                            collision.gameObject.GetComponent<SpaceStation.SpaceStation>();
                        station.TakeDamage(_shootingPirateData.GetLaserDamage());
                    }
                }
            }
            finally
            {
                // Always destroy the laser in the end
                Destroy(gameObject);
            }
        }

        public void SetShootingPirateData(PirateData shootingPirateData)
        {
            _shootingPirateData = shootingPirateData;
        }
    }
}
