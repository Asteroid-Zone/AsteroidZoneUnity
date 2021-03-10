using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndGame {
    public class VictoryMenu : MonoBehaviour {
    
        public AudioSource buttonPress;

        void Start()
        {
            // todo add game stats
        }

        public void BackToMenu() {
            buttonPress.Play();
            SceneManager.LoadScene(Scenes.MainMenuScene);
        }
    }
}
