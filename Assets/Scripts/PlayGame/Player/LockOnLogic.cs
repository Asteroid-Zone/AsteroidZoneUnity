using Photon.GameControllers;
using PlayGame.Camera;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayGame.Player {
    
    /// <summary>
    /// This class controls the lock on reticle.
    /// </summary>
    public class LockOnLogic : MonoBehaviour {
        
        private PlayerData _playerData;
        private MoveObject _moveObject;
        private Transform _lockTarget;
        private Image _image;
        public Sprite asteroidReticle;
        public Sprite enemyReticle;
        public Sprite outOfRangeReticle;
        public CameraManager cameraMan;
        
        private void Start() {
            GameObject player = (!DebugSettings.Debug && SceneManager.GetActiveScene().name != Scenes.TutorialScene) ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            
            _playerData = player.GetComponent<PlayerData>();
            if (_playerData.GetRole() == Role.StationCommander) Destroy(gameObject);

            _moveObject = player.GetComponent<MoveObject>();
            _image = GetComponent<Image>();
            _image.enabled = true;
        }

        /// <summary>
        /// Updates the position of the lock on reticle so that its on the lock target.
        /// <para>Sets the reticle image depending on the type of lock target.</para>
        /// Disables the reticle if there is no lock target.
        /// </summary>
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
