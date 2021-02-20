using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Statics;


namespace MainMenu
{
    public class MainMenuFunction : MonoBehaviourPunCallbacks
    {
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("The UI Panel with Main Menu Options")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;


        /// This client's version number. Users are separated from each other by gameVersion.
        string gameVersion = "1";

        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// This is used for the OnConnectedToMaster() callback when a user wants to leave the game.
        bool isConnecting;

        public AudioSource buttonPress;


        /// MonoBehaviour methods called on GameObject by Unity during early initialization phase.

        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        void Awake()
        {
            // Ensures PhotonNetwork.LoadLevel() can be used on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        //Methods overriding MonoBehaviourPunCallbacks Callbacks
        public override void OnConnectedToMaster()
        {
            Debug.Log("Asteroid Zone/MainMenuFunction: OnConnectedToMaster() was called by PUN");
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                // The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }

        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            isConnecting = false;
            Debug.LogWarningFormat("Asteroid Zone/MainMenuFunction: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Asteroid Zone/MainMenuFunction: OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Asteroid Zone/MainMenuFunction: OnJoinedRoom() called by PUN. Now this client is in a room.");
            // We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load PlayGame");

                // Load the Game.
                PhotonNetwork.LoadLevel("PlayGame");
            }
        }

        public void PlayGame()
        {
            buttonPress.Play();
            Connect();
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
