using Photon.GameControllers;
using PlayGame.Camera;
using PlayGame.Player.Movement;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.Player
{
    public class LockOnLogic : MonoBehaviour
    {
        private MoveObject _moveObject;
        private Transform _lockTarget;
        private Image image;
        public CameraManager cameraMan;

        // Start is called before the first frame update
        private void Start()
        {
            _moveObject = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar.GetComponent<MoveObject>() : TestPlayer.GetPlayerShip().GetComponent<MoveObject>();
            image = GetComponent<Image>();
            image.enabled = true;
        }

        // Update is called once per frame
        private void Update()
        {
            _lockTarget = _moveObject.GetLockTarget();
            if (_lockTarget)
            {
                Vector3 screenPos = cameraMan.GetCurrentCamera().WorldToScreenPoint(_lockTarget.position);
                image.transform.position = screenPos;
                image.enabled = true;
            }
            else
            {
                image.enabled = false;
            }
        }
    }
}
