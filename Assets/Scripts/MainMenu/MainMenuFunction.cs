using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class MainMenuFunction : MonoBehaviour
    {
        public AudioSource buttonPress;

        public void PlayGame()
        {
            buttonPress.Play();
            SceneManager.LoadScene("PlayGame");
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ViewCredits()
        {
            buttonPress.Play();
            SceneManager.LoadScene("Credits");
        }
    }
}
