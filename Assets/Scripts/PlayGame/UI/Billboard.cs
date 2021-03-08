using PlayGame.Camera;
using UnityEngine;

namespace PlayGame.UI
{
    public class Billboard : MonoBehaviour
    {
        private CameraManager _cameraManager;

        private void Start()
        {
            _cameraManager = FindObjectOfType<CameraManager>();
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + _cameraManager.GetCurrentCamera().transform.forward);
        }
    }
}
