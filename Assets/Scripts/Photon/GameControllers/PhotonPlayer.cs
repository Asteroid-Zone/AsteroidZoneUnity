using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonClass.GameController
{
    public class PhotonPlayer : MonoBehaviourPun
    {
        public GameObject myAvatar;

        public static PhotonPlayer PP;

        private void OnEnable()
        {
            if (PhotonPlayer.PP == null)
            {
                PhotonPlayer.PP = this;
            }
        }
    
        void Awake()
            {
                int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);
                if(this.photonView.IsMine)
                {
                    myAvatar = PhotonNetwork.Instantiate("PlayerShip", GameSetup.GS.spawnPoints[spawnPicker].position,
                        GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
                }
            }

    }
}