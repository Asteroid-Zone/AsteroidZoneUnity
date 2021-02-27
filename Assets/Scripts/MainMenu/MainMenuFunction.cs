using UnityEngine;
using UnityEngine.SceneManagement;
using Statics;
using PhotonClass;


namespace MainMenu
{
    public class MainMenuFunction : MonoBehaviour
    {


        public AudioSource buttonPress;




        public void PlayGame()
        {
            buttonPress.Play();
            PhotonClass.PhotonLobby.getInstance().Connect();
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
