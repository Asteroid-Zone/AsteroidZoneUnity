using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using Statics;


namespace MainMenu {
    public class MainMenuFunction : MonoBehaviour {
        
        public AudioSource buttonPress;
        
        public void PlayGame()
        {
            buttonPress.Play();
            PhotonLobby.getInstance().Connect();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ViewCredits()
        {
            buttonPress.Play();
            SceneManager.LoadScene(Scenes.CreditsScene);
        }
    }
}
