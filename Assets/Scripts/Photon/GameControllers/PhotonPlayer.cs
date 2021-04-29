using Photon.Pun;
using Statics;
using UnityEngine;

namespace Photon.GameControllers {
    
    /// <summary>
    /// This class represents a player in the PhotonNetwork game.
    /// </summary>
    public class PhotonPlayer : MonoBehaviourPun {
        
        public GameObject myAvatar;

        public static PhotonPlayer Instance;

        private void OnEnable() {
            if (Instance == null) {
                Instance = this;
            }
        }

        /// <remarks>This method can only be called if this instance belongs to the local player.</remarks>
        private void Awake() {
            if (!photonView.IsMine) return;
            int spawnPicker = Random.Range(0, GameSetup.Instance.spawnPoints.Length);
            
            // Instantiates the player as either the commander or a miner
            if (PhotonNetwork.IsMasterClient && !DebugSettings.SinglePlayer) myAvatar = PhotonNetwork.Instantiate(Prefabs.StationCommander, GameSetup.Instance.spawnPoints[spawnPicker].position, GameSetup.Instance.spawnPoints[spawnPicker].rotation);
            else myAvatar = PhotonNetwork.Instantiate(Prefabs.PlayerShip, GameSetup.Instance.spawnPoints[spawnPicker].position, GameSetup.Instance.spawnPoints[spawnPicker].rotation);
        }

    }
}