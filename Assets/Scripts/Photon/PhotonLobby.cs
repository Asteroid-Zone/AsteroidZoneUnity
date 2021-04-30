using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using MainMenu;
using UnityEngine;

namespace Photon {
    
    /// <summary>
    /// This class provides the LobbyControlPanel functions.
    /// </summary>
    public class PhotonLobby : MonoBehaviourPunCallbacks {
        
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("The UI Panel with Main Menu Options")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Panel with Lobby Options")]
        [SerializeField]
        private GameObject lobbyControlPanel;
        [Tooltip("The UI Panel with Room Options")]
        [SerializeField]
        private GameObject roomControlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;

        public string roomName;

        private static PhotonLobby _instance;

        public GameObject roomListPrefab;
        public GameObject playerListPrefab;

        public Transform roomPanel;
        public Transform playerPanel;
        public List<RoomInfo> GlobalRoomList;

        /// This client's version number. Users are separated from each other by gameVersion.
        private const string GameVersion = "1";

        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// This is used for the OnConnectedToMaster() callback when a user wants to leave the game.
        private bool _isConnecting;

        private void Awake() {
            _instance = this;
        }

        private void Start() {
            PhotonNetwork.GameVersion = GameVersion;
        }

        /// <summary> Reset the static instance. </summary>
        public static void ResetStaticVariables() {
            _instance = null;
        }

        /// <summary> Returns the lobby instance. </summary>
        public static PhotonLobby getInstance() {
            return _instance;
        }

        /// <summary>
        /// Start the connection process.
        /// <para>- If already connected, we attempt joining a random room.</para>
        /// <para>- If not yet connected, connect this application instance to Photon Cloud Network.</para>
        /// </summary>
        public void Connect() {
            // Hide the menu panels and show the loading screen
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            lobbyControlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected) {
                CreateRoom();
            } else {
                // Keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
                _isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = GameVersion;
            }
        }

        /// <summary>
        /// <para>Method overrides MonoBehaviourPunCallbacks.</para>
        /// //todo add description
        /// </summary>
        public override void OnConnectedToMaster() {
            Debug.Log("Asteroid Zone/MainMenuFunction: OnConnectedToMaster() was called by PUN");
            
            // Show the lobby panel and hide the loading screen
            progressLabel.SetActive(false);
            lobbyControlPanel.SetActive(true);
            
            PhotonNetwork.AutomaticallySyncScene = true;
            
            // We don't want to do anything if we are not attempting to join a room.
            // The case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (_isConnecting) {
                // The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                CreateRoom();
                _isConnecting = false;
            }
        }

        /// <summary>
        /// Creates a new PhotonRoom.
        /// </summary>
        void CreateRoom() {
          Debug.Log("Creating a room");
          RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = maxPlayersPerRoom};
          PhotonNetwork.CreateRoom(roomName, roomOps);
        }
        
        /// <summary>
        /// <para>Method overrides MonoBehaviourPunCallbacks.</para>
        /// Logs the error message and loads the lobby panel.
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnCreateRoomFailed(short returnCode, string message) {
            Debug.LogWarningFormat("Room creation failed: {0}", message);
            base.OnCreateRoomFailed(returnCode, message);
            
            progressLabel.SetActive(false);
            controlPanel.SetActive(false);
            lobbyControlPanel.SetActive(true);
            roomControlPanel.SetActive(false);
        }

        /// <summary>
        /// <para>Method overrides MonoBehaviourPunCallbacks.</para>
        /// Logs the reason for disconnecting and loads the main menu.
        /// </summary>
        /// <param name="cause"></param>
        public override void OnDisconnected(DisconnectCause cause) {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            _isConnecting = false;
            Debug.LogWarningFormat("Asteroid Zone/MainMenuFunction: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        /// <summary>
        /// <para>Method overrides MonoBehaviourPunCallbacks.</para>
        /// Updates the list of joinable rooms.
        /// </summary>
        /// <param name="roomList"></param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList) {
            foreach(RoomInfo room in roomList) { 
                ClearRoom(room);
                if(!room.RemovedFromList) {
                    ListRoom(room);
                }
            }
        }

        /// <summary>
        /// Destroys all the RoomListings.
        /// </summary>
        public void ClearList() {
            for (int i = roomPanel.childCount - 1; i >=0; i--) {
                Destroy(roomPanel.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Destroys the RoomListing for a given room.
        /// </summary>
        /// <param name="room"></param>
        public void ClearRoom(RoomInfo room) {
            for (int i = roomPanel.childCount - 1; i >= 0; i--) {
                if(roomPanel.GetChild(i).gameObject.GetComponent<RoomButton>().roomInfo.Name == room.Name) { 
                    Destroy(roomPanel.GetChild(i).gameObject);
                }
            }
        }

        /// <summary>
        /// Creates a new RoomListing from the RoomInfo.
        /// </summary>
        /// <param name="room"></param>
        public void ListRoom(RoomInfo room) { 
            Debug.Log("Listing room");
            if(room.IsOpen && room.IsVisible) {
                GameObject tempListing = Instantiate(roomListPrefab, roomPanel);
                RoomButton tempButton = tempListing.GetComponent<RoomButton>();
                tempButton.roomInfo = room;
                tempButton.SetRoom();
            }
        }

        /// <summary>
        /// Method is called when the player changes the text in the room name input box.
        /// </summary>
        /// <param name="roomNameInput"></param>
        public void RoomNameGrab(string roomNameInput) {
          roomName = roomNameInput;
        }

        /// <summary>
        /// <para>Method is called when the player presses the 'Find Games' button.</para>
        /// Clears the list of rooms and joins the lobby to receive a list of rooms.
        /// </summary>
        public void JoinLobby() {
          ClearList();
          PhotonNetwork.JoinLobby();
        }

    }

}
