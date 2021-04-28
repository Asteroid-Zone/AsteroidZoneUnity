using UnityEngine;

namespace Photon.GameControllers {
    
    /// <summary>
    /// This class is used to store the array of spawn points.
    /// </summary>
    public class GameSetup : MonoBehaviour {
        
        public static GameSetup Instance;

        public Transform[] spawnPoints;

        private void OnEnable() {
            if (Instance == null) {
                Instance = this;
            }
        }
    }
}
