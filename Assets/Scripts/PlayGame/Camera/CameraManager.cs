using PlayGame.Player;
using Statics;
using UnityEngine;

namespace PlayGame.Camera {
    
    /// <summary>
    /// This class manages which camera is active.
    /// </summary>
    public class CameraManager : MonoBehaviour {
        
        public UnityEngine.Camera followCamera, tacticalCamera;
        
        public GameObject playerPanel;
        public GameObject tacticalPanel;

        private bool _cockpitMode;

        private void Start() {
            // Set an initial mode depending of the role of the player
            SetMode(PlayerData.GetMyPlayerData().role != Role.StationCommander);
        }

        private void Update() {
            // If debug controls are enabled P switches camera mode
            if (DebugSettings.DebugKeys && Input.GetKeyDown(KeyCode.P)) {
                SetMode(!_cockpitMode); 
            }
        }

        /// <summary>
        /// Sets the active camera and enables the correct overlay.
        /// </summary>
        /// <param name="cockpit">true = cockpit camera, false = tactical camera</param>
        public void SetMode(bool cockpit) {
            _cockpitMode = cockpit;
            
            followCamera.enabled = cockpit;
            tacticalCamera.enabled = !cockpit;
            
            playerPanel.SetActive(cockpit);
            tacticalPanel.SetActive(!cockpit);
        }

        /// <summary>
        /// Returns the current active camera.
        /// </summary>
        public UnityEngine.Camera GetCurrentCamera() {
            return (_cockpitMode) ? followCamera : tacticalCamera;
        }
    }
}
