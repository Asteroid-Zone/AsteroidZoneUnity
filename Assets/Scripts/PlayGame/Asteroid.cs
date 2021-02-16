using UnityEngine;

namespace PlayGame {
    public class Asteroid : MonoBehaviour {

        private int _asteroidHealth;
        private void Start()
        {
            _asteroidHealth = 100;
        }

        public void MineAsteroid() {
            _asteroidHealth -= 5;

            if (_asteroidHealth <= 0) {
                Destroy(gameObject);
            }
        }

    }
}