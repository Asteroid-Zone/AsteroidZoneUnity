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
        private PlayerData _playerData;
        private MoveObject _moveObject;
        private Transform _lockTarget;
        private Image _image;
        public Sprite asteroidReticle;
        public Sprite enemyReticle;
        public Sprite outOfRangeReticle;
        public CameraManager cameraMan;

        // Start is called before the first frame update
        private void Start() {
            _playerData = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar.GetComponent<PlayerData>() : TestPlayer.GetPlayerShip().GetComponent<PlayerData>();
            if (_playerData.GetRole() == Role.StationCommander) Destroy(gameObject);
            
            _moveObject = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar.GetComponent<MoveObject>() : TestPlayer.GetPlayerShip().GetComponent<MoveObject>();
            _image = GetComponent<Image>();
            _image.enabled = true;
        }

        // Update is called once per frame
        private void Update() {
            _lockTarget = _moveObject.GetLockTarget();
            
            if (_lockTarget) {
                Vector3 screenPos = cameraMan.GetCurrentCamera().WorldToScreenPoint(_lockTarget.position);
                _image.transform.position = screenPos;
                
                ToggleCommand.LockTargetType lockType = _moveObject.GetLockType();
                if (_moveObject.InLockRange(lockType)) {
                    _image.sprite = lockType == ToggleCommand.LockTargetType.Asteroid ? asteroidReticle : enemyReticle;
                } else _image.sprite = outOfRangeReticle;

                _image.enabled = true;
            } else {
                _image.enabled = false;
            }
        }
    }
}
