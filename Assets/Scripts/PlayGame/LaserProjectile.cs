using UnityEngine;

namespace PlayGame {
    public class LaserProjectile : MonoBehaviour
    {

        private const int MaxRange = 20;
        
        private Vector3 _startPosition;

        private void Start() {
            _startPosition = transform.position;
        }

        private void Update() {
            if (Vector3.Distance(transform.position, _startPosition) > MaxRange) Destroy(gameObject); // Limit the lasers range
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag("Player")) return;
            if (collision.gameObject.CompareTag("Pirate")) Destroy(collision.gameObject); // TODO damage pirate instead of deleting it
            if (collision.gameObject.CompareTag("Asteroid")) {
                Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
                asteroid.MineAsteroid();
            }

            Destroy(gameObject);
        }
        
    }
}
