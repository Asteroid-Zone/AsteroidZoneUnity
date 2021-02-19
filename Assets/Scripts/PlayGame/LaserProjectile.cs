using UnityEngine;

namespace PlayGame {
    public class LaserProjectile : MonoBehaviour {

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
