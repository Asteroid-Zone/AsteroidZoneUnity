using UnityEngine;

namespace PlayGame {
    public class Asteroid : MonoBehaviour {
        
        private int _asteroidHealth;
        private int _totalResources;

        private const float MiningRate = 0.05f;
        private const int MaxHealth = 100;
        private const float MaxScale = 9f;
        private const float MinScale = 3f;

        private void Start()
        {
            _asteroidHealth = MaxHealth;
            _totalResources = Random.Range(25, 100);
        }

        public void MineAsteroid() {
            _asteroidHealth -= (int) (MaxHealth * MiningRate);
            float scale = ((MaxScale - MinScale) * ((float) _asteroidHealth / MaxHealth)) + MinScale;
            transform.localScale = new Vector3(scale, scale, scale);

            if (_asteroidHealth <= 0) {
                Destroy(gameObject);
            }
        }

        public int GetResources() {
            return (int) Mathf.Floor(_totalResources * MiningRate);
        }

    }
}