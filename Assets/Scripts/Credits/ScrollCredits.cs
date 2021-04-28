using System.Collections;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Credits {
    
    /// <summary>
    /// This class controls the Credits Scene.
    /// </summary>
    public class ScrollCredits : MonoBehaviour {
        
        public GameObject creditsRun;
    
        private void Start() {
            StartCoroutine(RollCredits());
        }

        private void Update() {
            if (Input.GetButtonDown("Cancel")) {
                SceneManager.LoadScene(Scenes.MainMenuScene);
            }
        }

        /// <summary>
        /// Plays the credits animation then loads the main menu scene.
        /// </summary>
        private IEnumerator RollCredits()
        {
            creditsRun.SetActive(true);

            yield return new WaitForSeconds(25);

            SceneManager.LoadScene(Scenes.MainMenuScene);
        }
    }
}
