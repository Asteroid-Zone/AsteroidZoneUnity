using Photon.Pun;
using PlayGame.Player;
using Statics;
using UnityEngine;

namespace PlayGame.Pirates {

    public class PirateLaserProjectile : MonoBehaviour {

        private Vector3 _startPosition;

        private PirateData _shootingPirateData;

        private void Start() {
            _startPosition = transform.position;
        }

        private void Update() {
            transform.Translate(-transform.forward * (_shootingPirateData.GetLaserSpeed() * Time.deltaTime));
            if (Vector3.Distance(transform.position, _startPosition) > _shootingPirateData.GetLaserRange()) Destroy(gameObject); // Limit the lasers range
        }

        private void OnTriggerEnter(Collider collision) {
            try
            {
                // todo play animation (explosion)
                
                if (collision.gameObject.CompareTag(Tags.PlayerTag))
                {
                    PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
                    playerData.TakeDamage(_shootingPirateData.GetLaserDamage());
                }

                if (collision.gameObject.CompareTag(Tags.AsteroidTag))
                {
                    Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
                    asteroid.MineAsteroid(GameConstants.PirateLaserMiningRate, null);
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
