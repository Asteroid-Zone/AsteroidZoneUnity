﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Statics;
using PlayGame.Player;

namespace PlayGame.UI {
    public class GameManager : MonoBehaviourPunCallbacks {

        public static bool gameOver = false;

        /// Called when the local player left the room. We need to load the launcher scene.
        public override void OnLeftRoom() {
            SceneManager.LoadScene(Scenes.MainMenuScene);
        }

        public static void LeaveRoom() {
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
