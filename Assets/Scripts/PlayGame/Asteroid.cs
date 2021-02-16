using System.Collections;
using UnityEngine;

namespace PlayGame {
    public class Asteroid : MonoBehaviour {
        
        private int _asteroidHealth;
        private int _totalResources;

        private const float MiningRate = 0.05f;
        private const int MaxHealth = 100;
        
        private const float MaxScale = 9f;
        private const float MinScale = 3f;

        private void Start() {
            _asteroidHealth = MaxHealth;
            _totalResources = Random.Range(25, 100); // Sets the asteroids resources to a random number between 25 and 100
        }
        
        private IEnumerator FadeOutAsteroid() {
            const float fadeSpeed = 1f;
            Material material = GetComponent<Renderer>().material;
            
            while (material.color.a > 0) {
                Color c = material.color;
                float fadeAmount = c.a - (fadeSpeed * Time.deltaTime);

                c = new Color(c.r, c.g, c.b, fadeAmount);
                material.color = c;
                yield return null;
            }
            
            Destroy(gameObject); // Delete the game object when its faded
        }

        public void MineAsteroid() {
            _asteroidHealth -= (int) (MaxHealth * MiningRate);
            float scale = ((MaxScale - MinScale) * ((float) _asteroidHealth / MaxHealth)) + MinScale; // Calculate the asteroids size based on the amount the asteroid has been mined
            transform.localScale = new Vector3(scale, scale, scale);

            if (_asteroidHealth <= 0) {
                StartCoroutine(FadeOutAsteroid());
            }
        }

        public int GetResources() {
            return (int) Mathf.Floor(_totalResources * MiningRate);
        }

    }
}