using PlayGame.Player;
using UnityEngine;

namespace PlayGame.Pirates {

    public class PirateLaserProjectile : MonoBehaviour
    {
        private const int MaxRange = 10;
        
        private Vector3 _startPosition;

        private void Start() {
            _startPosition = transform.position;
        }

        private void Update() {
            if (Vector3.Distance(transform.position, _startPosition) > MaxRange) Destroy(gameObject); // Limit the lasers range
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag("Pirate"))
            {
                return;
            }
            
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerData playerData = collision.gameObject.GetComponent<PlayerData>();
                playerData.TakeDamage(10);
            }
            
            if (collision.gameObject.CompareTag("Asteroid")) {
                Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
                asteroid.MineAsteroid();
            }

            Destroy(gameObject);
        }
    }
}
