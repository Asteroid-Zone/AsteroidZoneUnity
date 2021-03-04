using Photon.Pun;
using System.Collections;
using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame {
    public class Asteroid : MonoBehaviour {

        private float _initialScale;
        private int _resourcesRemaining;
        private int _totalResources;

        private bool _asteroidDestroyed = false;

        private const float MaxScale = 9f;
        private const float MinScale = 2f;

        private const int MinResources = 25;
        private const int MaxResources = 100;
        
        private const float FadeSpeed = 2f;

        private void Start() {
            _totalResources = Random.Range(MinResources, MaxResources);
            _resourcesRemaining = _totalResources;
            _initialScale = MaxScale * ((float) _totalResources / MaxResources); // Initial size of the asteroid
            transform.localScale = new Vector3(_initialScale, _initialScale, _initialScale);
        }
        
        private IEnumerator FadeOutAsteroid() {
            Material material = GetComponent<Renderer>().material;
            
            while (material.color.a > 0) {
                Color c = material.color;
                float fadeAmount = c.a - (FadeSpeed * Time.deltaTime);

                c = new Color(c.r, c.g, c.b, fadeAmount);
                material.color = c;
                yield return null;
            }
            
            if(!DebugSettings.Debug) PhotonNetwork.Destroy(gameObject); // Delete the game object when its faded
            else Destroy(gameObject);
        }

        public void MineAsteroid(int miningRate) {
            _resourcesRemaining -= miningRate;
            if (_resourcesRemaining < 0) _resourcesRemaining = 0;
            
            float scale = _initialScale * ((float) _resourcesRemaining / _totalResources); // Calculate the asteroids size based on the amount the asteroid has been mined
            if (scale < MinScale) scale = MinScale; // Asteroid is always bigger than minimum scale
            transform.localScale = new Vector3(scale, scale, scale);

            if (_resourcesRemaining <= 0 && !_asteroidDestroyed) {
                _asteroidDestroyed = true;
                EventsManager.AddMessageToQueue("Asteroid destroyed at " + GridCoord.GetCoordFromVector(transform.position));
                StartCoroutine(FadeOutAsteroid());
            }
        }

        public int GetResources(int miningRate) {
            if (_resourcesRemaining > miningRate) return miningRate;
            return _resourcesRemaining;
        }

    }
}