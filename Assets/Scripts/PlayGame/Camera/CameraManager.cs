using UnityEngine;

namespace PlayGame.Camera
{
    public class CameraManager : MonoBehaviour
    {
        public UnityEngine.Camera followCamera, tacticalCamera;
        
        public GameObject playerPanel;
        public GameObject tacticalPanel;

        private bool _cockpitMode;

        private void Update() {
            if (!Input.GetKeyDown(KeyCode.P)) return;
            SetMode(!_cockpitMode); // Switch mode
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
