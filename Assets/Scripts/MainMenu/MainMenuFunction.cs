using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Statics;


namespace MainMenu {
    public class MainMenuFunction : MonoBehaviourPunCallbacks {

        [Tooltip("The UI Panel with Main Menu Options")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Panel with Lobby Options")]
        [SerializeField]
        private GameObject lobbyControlPanel;
        [SerializeField]
        private GameObject roomControlPanel;
        [SerializeField]
        private GameObject progressLabel;

        public AudioSource buttonPress;

        void Start() {
            if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect(); // Need to disconnect otherwise the lobby will load before the player presses play game
            
            controlPanel.SetActive(true);
            progressLabel.SetActive(false);
            lobbyControlPanel.SetActive(false);
            roomControlPanel.SetActive(false);
        }

        void Update() {
			if (lobbyControlPanel.activeSelf && Input.GetButtonDown("Cancel")) {
				lobbyControlPanel.SetActive(false);
				controlPanel.SetActive(true);
			}
        }

        public void PlayGame() {
            buttonPress.Play();
            PhotonNetwork.ConnectUsingSettings();
            controlPanel.SetActive(false);
            progressLabel.SetActive(true);
        }

        public void JoinLobby() {
          buttonPress.Play();
          lobbyControlPanel.SetActive(false);
        }

        public void QuitGame() {
            Application.Quit();
        }

        public void ViewCredits() {
            buttonPress.Play();
            SceneManager.LoadScene(Scenes.CreditsScene);
        }
    }
}
