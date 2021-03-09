using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame {
    public class Asteroid : MonoBehaviour {
        
        private static readonly List<Mesh> AsteroidMeshes = new List<Mesh>();

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
            if (AsteroidMeshes.Count == 0) {
                AsteroidMeshes.Add(Resources.Load<Mesh>("Models/asteroid_1"));
                AsteroidMeshes.Add(Resources.Load<Mesh>("Models/asteroid_2"));    
            }

            int asteroidMeshIndex = Random.Range(0, AsteroidMeshes.Count);
            GetComponent<MeshFilter>().mesh = AsteroidMeshes[asteroidMeshIndex];
            GetComponent<MeshCollider>().sharedMesh = AsteroidMeshes[asteroidMeshIndex];
        
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
            
            if(!DebugSettings.Debug && PhotonNetwork.IsMasterClient) PhotonNetwork.Destroy(gameObject); // Delete the game object when its faded
            else if (DebugSettings.Debug) Destroy(gameObject);
        }

        public void MineAsteroid(int miningRate) {
            _resourcesRemaining -= miningRate;
            if (_resourcesRemaining < 0) _resourcesRemaining = 0;
            
            float scale = _initialScale * ((float) _resourcesRemaining / _totalResources); // Calculate the asteroids size based on the amount the asteroid has been mined
            if (scale < MinScale) scale = MinScale; // Asteroid is always bigger than minimum scale
            transform.localScale = new Vector3(scale, scale, scale);

            if (_resourcesRemaining <= 0 && !_asteroidDestroyed) {
                _asteroidDestroyed = true;
                EventsManager.AddMessage("Asteroid destroyed at " + GridCoord.GetCoordFromVector(transform.position));
                StartCoroutine(FadeOutAsteroid());
            }
        }

        public int GetResources(int miningRate) {
            if (_resourcesRemaining > miningRate) return miningRate;
            return _resourcesRemaining;
        }

    }
}