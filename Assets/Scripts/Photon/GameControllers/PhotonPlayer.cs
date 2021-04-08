using Photon.Pun;
using Statics;
using UnityEngine;

namespace Photon.GameControllers
{
    public class PhotonPlayer : MonoBehaviourPun
    {
        public GameObject myAvatar;

        public static PhotonPlayer Instance;

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Awake()
            {
                int spawnPicker = Random.Range(0, GameSetup.Instance.spawnPoints.Length);
                if(photonView.IsMine) {
                    if (PhotonNetwork.IsMasterClient && !DebugSettings.SinglePlayer) myAvatar = PhotonNetwork.Instantiate(Prefabs.StationCommander, GameSetup.Instance.spawnPoints[spawnPicker].position, GameSetup.Instance.spawnPoints[spawnPicker].rotation, 0);
                    else myAvatar = PhotonNetwork.Instantiate(Prefabs.PlayerShip, GameSetup.Instance.spawnPoints[spawnPicker].position, GameSetup.Instance.spawnPoints[spawnPicker].rotation, 0);
                }
            }

    }
}