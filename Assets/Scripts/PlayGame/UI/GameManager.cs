using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using PlayGame.Pirates;
using Statics;
using PlayGame.Player;
using PlayGame.Stats;

namespace PlayGame.UI {
    public class GameManager : MonoBehaviourPunCallbacks {

        public static bool gameOver = false;

        private void Start() {
            ResetStaticVariables();
        }

        private static void ResetStaticVariables() {
            gameOver = false;
            PirateController.ResetStaticVariables();
            PlayerData.Players = new List<GameObject>();
            StatsManager.PlayerStatsList.Clear();
            StatsManager.GameStats.Reset();
        }

        /// Called when the local player left the room. We need to load the launcher scene.
        public override void OnLeftRoom() {
            CleanUpGameObjects();
            SceneManager.LoadScene(Scenes.MainMenuScene);
            
            base.OnLeftRoom();
        }
        
        private void CleanUpGameObjects() {
            foreach (GameObject g in FindObjectsOfType<GameObject>()) {
                if (!g.name.Equals("PhotonMono")) Destroy(g);
            }
        }

        public void LeaveRoom() {
            PhotonNetwork.LeaveRoom();
        }

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Play Game");
            PhotonNetwork.LoadLevel(Scenes.PlayGameScene);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        }


        public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
        {
            PlayerData.UpdatePlayerLists();
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        }


    }
}
