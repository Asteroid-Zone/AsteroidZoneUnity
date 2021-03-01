using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonClass.GameController
{
    public class PhotonPlayer : MonoBehaviour
    {
        private PhotonView PV;
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
                PV = GetComponent<PhotonView>();
                int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);
                if(PV.IsMine)
                {
                    myAvatar = PhotonNetwork.Instantiate("PlayerShip", GameSetup.GS.spawnPoints[spawnPicker].position,
                        GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
                }
            }


        public bool IsMine()
        {
            return (PV.IsMine);
        }
    }
}