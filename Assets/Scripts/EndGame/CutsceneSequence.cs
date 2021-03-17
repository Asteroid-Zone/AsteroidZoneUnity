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
    
        private void Start()
        {
            StartCoroutine(StatsManager.GameStats.victory ? VictorySequence() : DefeatSequence());
        }

        private IEnumerator VictorySequence()
        {
            gameObject.GetComponent<Animator>().Play("StationFlyby");
            yield return new WaitForSeconds(5);
            _trackingSpaceStation = false;
            gameObject.GetComponent<Animator>().Play("StationHyperspace");
            yield return new WaitForSeconds(0.5f);
            spaceStation.gameObject.SetActive(false);
            SceneManager.LoadScene(Scenes.VictoryScene);
        }

        private IEnumerator DefeatSequence()
        {
            gameObject.GetComponent<Animator>().Play("StationFlyby");
            yield return new WaitForSeconds(5);
            _trackingSpaceStation = false;
            gameObject.GetComponent<Animator>().Play("StationExplode");
            yield return new WaitForSeconds(0.5f);
            spaceStation.gameObject.SetActive(false);
            SceneManager.LoadScene(Scenes.VictoryScene);
        }

        private void Update()
        {
            if (_trackingSpaceStation) camera.transform.LookAt(spaceStation);
        }
    }
}
