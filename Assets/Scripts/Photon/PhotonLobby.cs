using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon
{
    public class PhotonLobby : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
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

        public string RoomName;

        private static PhotonLobby _instance;

        public GameObject roomListPrefab;
        public GameObject playerListPrefab;

        public Transform roomPanel;
        public Transform playerPanel;
        public List<RoomInfo> globalRoomList;

        /// This client's version number. Users are separated from each other by gameVersion.
        private const string GameVersion = "1";

        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// This is used for the OnConnectedToMaster() callback when a user wants to leave the game.
        private bool _isConnecting;

        private bool connected;

        /// MonoBehaviour methods called on GameObject by Unity during early initialization phase.

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            PhotonNetwork.GameVersion = GameVersion;
            connected = false;
        }

        public static PhotonLobby getInstance()
        {
            return _instance;
        }


        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            lobbyControlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected)
            {
                CreateRoom();
            }
            else
            {
                // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
                _isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = GameVersion;
            }
            progressLabel.SetActive(false);
            roomControlPanel.SetActive(true);
        }

        //Methods overriding MonoBehaviourPunCallbacks Callbacks
        public override void OnConnectedToMaster()
        {
            progressLabel.SetActive(false);
            lobbyControlPanel.SetActive(true);
            Debug.Log("Asteroid Zone/MainMenuFunction: OnConnectedToMaster() was called by PUN");
            PhotonNetwork.AutomaticallySyncScene = true;
            // we don't want to do anything if we are not attempting to join a room.
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (_isConnecting)
            {
                // The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                CreateRoom();
                _isConnecting = false;
            }

        }

        void CreateRoom()
        {
          Debug.Log("creating a room");
          RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = maxPlayersPerRoom};
          PhotonNetwork.CreateRoom(RoomName, roomOps);
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            _isConnecting = false;
            Debug.LogWarningFormat("Asteroid Zone/MainMenuFunction: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
          foreach(RoomInfo room in roomList)
          {
            if(!room.RemovedFromList)
            {
              ListRoom(room);
            }
            else
            {
              ClearRoom(room);
            }
          }
        }

        public void ClearList()
        {
          for (int i = roomPanel.childCount - 1; i >=0; i--)
          {
            Destroy(roomPanel.GetChild(i).gameObject);
          }
        }

        public void ClearRoom(RoomInfo room)
        {
          for (int i = roomPanel.childCount - 1; i >= 0; i--)
          {
            if(roomPanel.GetChild(i).gameObject.GetComponent<RoomButton>().roomName == room.Name)
            {
              Destroy(roomPanel.GetChild(i).gameObject);
            }
          }
        }

        public void ListRoom(RoomInfo room)
        {
          if(room.IsOpen && room.IsVisible)
          {
            GameObject tempListing = Instantiate(roomListPrefab, roomPanel);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            tempButton.SetRoom();
          }
        }


        public void RoomNameGrab(string roomNameInput)
        {
          RoomName = roomNameInput;
        }

        public void JoinLobby()
        {
          ClearList();
          PhotonNetwork.JoinLobby();
        }

    }

}
