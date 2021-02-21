using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Statics;


namespace MainMenu
{
    public class MainMenuFunction : MonoBehaviourPunCallbacks
    {
      public AudioSource buttonPress;


        public void PlayGame()
        {
            buttonPress.Play();
            SceneManager.LoadScene(Scenes.LobbyControllerScene);
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
