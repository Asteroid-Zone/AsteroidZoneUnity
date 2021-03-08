using UnityEngine;

namespace PlayGame.Camera
{
    public class CameraManager : MonoBehaviour
    {
        public UnityEngine.Camera followCamera, tacticalCamera;

        private void Start()
        {
            followCamera.enabled = true;
            tacticalCamera.enabled = false;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.P)) return;
            followCamera.enabled = !followCamera.enabled;
            tacticalCamera.enabled = !tacticalCamera.enabled;
        }

        public UnityEngine.Camera GetCurrentCamera()
        {
            return (followCamera.enabled) ? followCamera : tacticalCamera;
        }
    }
}
