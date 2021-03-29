using Photon.GameControllers;
using PlayGame.Camera;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
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
        public Sprite asteroidReticle;
        public Sprite enemyReticle;
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
                ToggleCommand.LockTargetType _lockType = _moveObject.GetLockType();
                Vector3 screenPos = cameraMan.GetCurrentCamera().WorldToScreenPoint(_lockTarget.position);
                image.transform.position = screenPos;
                if (_lockType == ToggleCommand.LockTargetType.Asteroid)
                {
                    image.sprite = asteroidReticle;
                }
                else
                {
                    image.sprite = enemyReticle;
                }
                image.enabled = true;
            }
            else
            {
                image.enabled = false;
            }
        }
    }
}
