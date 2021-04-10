using Photon.Pun;
using Photon.Realtime;
using PlayGame.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Photon
{
    public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        [Tooltip("The UI Panel with Room Options")]
        [SerializeField]
        private GameObject roomControlPanel;
        [Tooltip("The UI Panel with lobby Options")]
        [SerializeField]
        private GameObject lobbyControlPanel;
        private GameObject progressLabel;
        public GameObject startButton;

        public GameObject playerListPrefab;
        public Transform playerPanel;

        public static PhotonRoom Instance;
        private PhotonView PV;

        public int multiplayerScene;
        private int _currentScene;

        private void Awake()
        {
            //Set up Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(Instance.gameObject);
                    Instance = this;
                }
            }
            DontDestroyOnLoad(gameObject);
            PV = GetComponent<PhotonView>();
        }

        public override void OnEnable()
        {
            //subscribe to functions
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
            SceneManager.sceneLoaded += OnSceneFinishedLoading;
        }

        public override void OnDisable()
        {
            //unsubscribe to functions
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
            SceneManager.sceneLoaded -= OnSceneFinishedLoading;
        }

        public override void OnJoinedRoom()
        {
            //sets player data when we join the room
            base.OnJoinedRoom();
            lobbyControlPanel.SetActive(false);
            roomControlPanel.SetActive(true);
            Debug.Log("Asteroid Zone/MainMenuFunction: OnJoinedRoom() called by PUN. Now this client is in a room.");
            if(PhotonNetwork.IsMasterClient)
            {
              startButton.SetActive(true);
            }
            else
            {
              startButton.SetActive(false);
            }
            ClearList();
            ListLobby();
        }

        public override void OnPlayerEnteredRoom(Player player)
        {
            base.OnPlayerEnteredRoom(player);
            Debug.Log("happening>?");
            ClearList();
            ListLobby();
        }

        public override void OnPlayerLeftRoom(Player player)
        {
          base.OnPlayerLeftRoom(player);
          Debug.Log("Player left room");
          ClearList();
          ListLobby();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
          PhotonNetwork.LeaveRoom();
          roomControlPanel.SetActive(false);
          lobbyControlPanel.SetActive(true);
        }

        public void StartGame()
        {
            Debug.Log("Loading Level");
            PhotonNetwork.LoadLevel(multiplayerScene);
            StatsManager.GameStats.startTime = Time.time;
        }

        private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            _currentScene = scene.buildIndex;
            if (_currentScene == multiplayerScene)
            {
                CreatePlayer();
            }
        }

        private static void CreatePlayer() {
            PhotonNetwork.Instantiate("PhotonNetworkPlayer", new Vector3(5f,0f,10f) , Quaternion.identity);
        }

        public void ClearList() {
            if (playerPanel == null) return;

            for (int i = playerPanel.childCount - 1; i >=0; i--) {
                Destroy(playerPanel.GetChild(i).gameObject);
            }
        }


        public void ListLobby()
        {
          if(PhotonNetwork.InRoom)
          {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                GameObject tempListing = Instantiate(playerListPrefab, playerPanel);
                Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                tempText.text = player.NickName;
            }
          }
        }
    }
}
