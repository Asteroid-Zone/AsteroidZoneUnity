using UnityEngine;

namespace PlayGame.Player {
  
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
            if (collision.gameObject.CompareTag("Pirate")) {
                EventsManager.AddMessageToQueue("Pirate destroyed at " + GridCoord.GetCoordFromVector(collision.gameObject.transform.position));
                Destroy(collision.gameObject); // TODO damage pirate instead of deleting it
            }
            
            if (collision.gameObject.CompareTag("Asteroid")) {
                Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
                asteroid.MineAsteroid();
            }

            Destroy(gameObject);
        }
    }
}
