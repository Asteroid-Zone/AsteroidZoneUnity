using Photon.Pun;
using PlayGame.Stats;
using UnityEngine;
using UnityEngine.SceneManagement;
using Statics;

namespace MainMenu {
    
    /// <summary>
    /// This class provides the main menu functions.
    /// </summary>
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

        private void Start() {
            if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect(); // Need to disconnect otherwise the lobby will load before the player presses play game
            
            controlPanel.SetActive(true);
            progressLabel.SetActive(false);
            lobbyControlPanel.SetActive(false);
            roomControlPanel.SetActive(false);
        }

        private void Update() {
            // If the player presses cancel in the lobby selection screen load the main menu
			if (lobbyControlPanel.activeSelf && Input.GetButtonDown("Cancel")) {
				lobbyControlPanel.SetActive(false);
				controlPanel.SetActive(true);
                if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
			}
        }

        /// <summary>
        /// <para>Method is called when the player presses the 'Play Game' button on the main menu.</para>
        /// Starts connecting to the PhotonNetwork and displays a loading screen.
        /// </summary>
        public void PlayGame() {
            buttonPress.Play();
            PhotonNetwork.ConnectUsingSettings();
            controlPanel.SetActive(false);
            progressLabel.SetActive(true);
        }

        /// <summary>
        /// <para>Method is called when the player presses the 'Play Tutorial' button on the main menu.</para>
        /// Loads the tutorial scene.
        /// </summary>
        public void PlayTutorial() {
            buttonPress.Play();
            DebugSettings.Debug = true;
            StatsManager.GameStats.startTime = Time.time;
            SceneManager.LoadScene(Scenes.TutorialScene);
        }

        /// <summary>
        /// <para>Method is called when the player presses the 'View Credits' button on the main menu.</para>
        /// Loads the credits scene.
        /// </summary>
        public void ViewCredits() {
            buttonPress.Play();
            SceneManager.LoadScene(Scenes.CreditsScene);
        }
    }
}
