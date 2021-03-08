using System.Collections;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Credits
{
    public class ScrollCredits : MonoBehaviour
    {
        public GameObject creditsRun;
    
        private void Start()
        {
            StartCoroutine(RollCredits());
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                SceneManager.LoadScene(Scenes.MainMenuScene);
            }
        }

        private IEnumerator RollCredits()
        {
            creditsRun.SetActive(true);

            yield return new WaitForSeconds(25);

            SceneManager.LoadScene(Scenes.MainMenuScene);
        }
    }
}
