using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using Statics;


namespace MainMenu {
    public class MainMenuFunction : MonoBehaviour {

        [Tooltip("The UI Panel with Main Menu Options")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Panel with Lobby Options")]
        [SerializeField]
        private GameObject lobbyControlPanel;
        [SerializeField]
        private GameObject roomControlPanel;

        public AudioSource buttonPress;

        void Start()
        {
          controlPanel.SetActive(true);
          lobbyControlPanel.SetActive(false);
          roomControlPanel.SetActive(false);
        }

		void Update() 
		{
			if (lobbyControlPanel.activeSelf && Input.GetButtonDown("Cancel")) 
			{
				lobbyControlPanel.SetActive(false);
				controlPanel.SetActive(true);
			}
		
		}

        public void PlayGame()
        {
            buttonPress.Play();
            controlPanel.SetActive(false);
            lobbyControlPanel.SetActive(true);
        }

        public void JoinLobby()
        {
          buttonPress.Play();
          lobbyControlPanel.SetActive(false);
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
