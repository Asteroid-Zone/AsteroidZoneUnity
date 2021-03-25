using Photon.Pun;
using Photon.Realtime;
using PlayGame.Stats;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon
{
    public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
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
            Debug.Log("Asteroid Zone/MainMenuFunction: OnJoinedRoom() called by PUN. Now this client is in a room.");
            if (!PhotonNetwork.IsMasterClient) return;
            StartGame();
        }

        private void StartGame()
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
    }
}
