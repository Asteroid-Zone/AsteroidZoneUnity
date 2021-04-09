using PlayGame.Player;
using Statics;
using UnityEngine;

namespace PlayGame.Camera
{
    public class CameraManager : MonoBehaviour
    {
        public UnityEngine.Camera followCamera, tacticalCamera;
        
        public GameObject playerPanel;
        public GameObject tacticalPanel;

        private bool _cockpitMode;

        private void Start()
        {
            // Set an initial mode depending of the role of the player
            SetMode(PlayerData.GetMyPlayerData().role != Role.StationCommander);
        }

        private void Update() {
            if (DebugSettings.DebugKeys && Input.GetKeyDown(KeyCode.P))
            {
                SetMode(!_cockpitMode); 
            }
        }

        public void SetMode(bool cockpit) {
            _cockpitMode = cockpit;
            
            followCamera.enabled = cockpit;
            tacticalCamera.enabled = !cockpit;
            
            playerPanel.SetActive(cockpit);
            tacticalPanel.SetActive(!cockpit);
        }

        public UnityEngine.Camera GetCurrentCamera() {
            return (_cockpitMode) ? followCamera : tacticalCamera;
        }
    }
}
