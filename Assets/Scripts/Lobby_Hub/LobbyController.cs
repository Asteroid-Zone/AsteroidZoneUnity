using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Statics;

namespace MainMenu
{
    /// We require the room code provided.
    public class LobbyController : MonoBehaviourPunCallbacks
    {
      [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
      [SerializeField]
      private byte maxPlayersPerRoom = 4;
      [Tooltip("The UI Panel with Main Menu Options")]
      [SerializeField]
      private GameObject controlPanelLobby;
      [Tooltip("The UI Label to inform the user that the connection is in progress")]
      [SerializeField]
      private GameObject progressLabelLobby;
      /// This client's version number. Users are separated from each other by gameVersion.
      string gameVersion = "1";

      /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
      /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
      /// This is used for the OnConnectedToMaster() callback when a user wants to leave the game.
      bool isConnecting;
      bool isJoining;

      public AudioSource buttonPress;

    // Start is called before the first frame update
    void Start()
    {
      progressLabelLobby.SetActive(false);
      controlPanelLobby.SetActive(true);
    }

    void Awake()
    {
        // Ensures PhotonNetwork.LoadLevel() can be used on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    ///Joining a game
    public void ConnectGame()
    {
      progressLabelLobby.SetActive(true);
      controlPanelLobby.SetActive(false);
      if(PhotonNetwork.IsConnected)
      {
        PhotonNetwork.JoinRoom("Test");
        Debug.Log("Room Test Joined");
      }
      else
      {
        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        isJoining = PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
      }

    }


    /// Start the connection process.
    /// - If already connected, we create the room as designated by the input
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    public void Connect()
    {
        progressLabelLobby.SetActive(true);
        controlPanelLobby.SetActive(false);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.CreateRoom("Test", new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            Debug.Log("Room created");
        }
        else
        {
            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
      }

      public override void OnConnectedToMaster()
      {
        Debug.Log("Asteroid Zone/LobbyController: OnConnectedToMaster() was called by PUN");
        if(isConnecting)
        {
          Connect();
          isConnecting = false;
        }
        if(isJoining)
        {
          Debug.Log("now calling connectgame()");
          ConnectGame();
          isJoining = false;
        }
      }



      //if join room fails this function is called for error catching
      public override void OnJoinRoomFailed(short returnCode, string message)
      {
        Debug.Log("Joining room failed please try again. Reason being");
        Debug.Log(message);
      }

      public override void OnDisconnected(DisconnectCause cause)
      {
          progressLabelLobby.SetActive(false);
          controlPanelLobby.SetActive(true);
          isConnecting = false;
          Debug.LogWarningFormat("Asteroid Zone/MainMenuFunction: OnDisconnected() was called by PUN with reason {0}", cause);
      }

    public override void OnJoinedRoom()
      {
          Debug.Log("Asteroid Zone/MainMenuFunction: OnJoinedRoom() called by PUN. Now this client is in a room.");
          // We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
          if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
          {
              Debug.Log("We load PlayGame");
                // Load the Game.
              PhotonNetwork.LoadLevel("Lobby");
          }
      }

    public void CreateGame()
      {
          buttonPress.Play();
          Connect();
      }

    public void JoinGame()
      {
          buttonPress.Play();
          ConnectGame();
      }
  }
}
