using System.Collections;
using PlayGame.Stats;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndGame
{
    public class CutsceneSequence : MonoBehaviour
    {
        public Transform spaceStation;
        public new Camera camera;

        private bool _trackingSpaceStation = true;
        private Animator _animator;

        private void Start()
        {
            _animator = gameObject.GetComponent<Animator>();
            StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            _animator.Play("Camera.FadeIn");
            _animator.Play("Station.Flyby");
            yield return new WaitForSeconds(5);
            _trackingSpaceStation = false;
            _animator.Play(StatsManager.GameStats.victory ? "Station.Hyperspace" : "Station.Explode");
            yield return new WaitForSeconds(0.5f);
            spaceStation.gameObject.SetActive(false);
            _animator.Play("Camera.FadeOut");
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(Scenes.VictoryScene);
        }

        private void Update()
        {
            if (_trackingSpaceStation) camera.transform.LookAt(spaceStation);
        }
    }
}
