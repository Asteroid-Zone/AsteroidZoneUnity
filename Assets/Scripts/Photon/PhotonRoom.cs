using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PhotonClass
{
    public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        public static PhotonRoom room;
        private PhotonView PV;
        
        public int multiplayerScene;
        private int currentScene;

        void Awake()
        {
            //Set up Singleton
            if (PhotonRoom.room == null)
            {
                PhotonRoom.room = this;
            }
            else
            {
                if (PhotonRoom.room != this)
                {
                    Destroy(PhotonRoom.room.gameObject);
                    PhotonRoom.room = this;
                }
            }
            DontDestroyOnLoad(this.gameObject);
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
            Debug.Log("Asteroid Zone/MainMenuFunction: OnJoinedRoom() called by PUN. Now this client is in a room.");
            if (!PhotonNetwork.IsMasterClient) return;
            StartGame();
        }

        void StartGame()
        {
            Debug.Log("Loading Level");
            PhotonNetwork.LoadLevel(multiplayerScene);
        }

        void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            currentScene = scene.buildIndex;
            if (currentScene == multiplayerScene)
            {
                CreatePlayer();
            }
        }

        private void CreatePlayer()
        {

            PhotonNetwork.Instantiate("PhotonNetworkPlayer", new Vector3(5f,0f,10f) , Quaternion.identity, 0);
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
