using System;
using System.Collections;
using PlayGame;
using PlayGame.Stats;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndGame {
    
    /// <summary>
    /// This class controls the GameEndCutscene Scene.
    /// </summary>
    public class CutsceneSequence : MonoBehaviour {
        
        public Transform spaceStation;
        public new Camera camera;
        public GameObject explosionPrefab;

        private bool _trackingSpaceStation = true;
        private Animator _animator;

        private void Start() {
            _animator = gameObject.GetComponent<Animator>();
            // Hides the player models
            var players = GameObject.FindGameObjectsWithTag(Tags.PlayerTag);
            foreach (var player in players) {
                player.SetActive(false);
            }
            
            StartCoroutine(PlaySequence());
        }

        /// <summary>
        /// Plays the correct end cutscene animation based on the <c>GameOverType</c> stored in <c> StatsManager.GameStats.gameOverType</c>, then loads the victory scene.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when there is an invalid game over type</exception>
        private IEnumerator PlaySequence() {
            _animator.Play("Camera.FadeIn");
            _animator.Play("Station.Flyby");
            yield return new WaitForSeconds(5);
            _trackingSpaceStation = false;
            
            switch (StatsManager.GameStats.gameOverType) {
                case GameManager.GameOverType.Victory:
                    _animator.Play("Station.Hyperspace");
                    break;
                case GameManager.GameOverType.StationDestroyed:
                    Instantiate(explosionPrefab, spaceStation, false);
                    spaceStation.gameObject.GetComponent<Animation>().Play("Explode");
                    break;
                case GameManager.GameOverType.TimeUp:
                    // todo add pirate boss destroying station
                    Instantiate(explosionPrefab, spaceStation, false);
                    spaceStation.gameObject.GetComponent<Animation>().Play("Explode");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            yield return new WaitForSeconds(1.5f);
            _animator.Play("Camera.FadeOut");
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(Scenes.VictoryScene);
        }

        private void Update() {
            if (_trackingSpaceStation) camera.transform.LookAt(spaceStation);
        }
    }
}
