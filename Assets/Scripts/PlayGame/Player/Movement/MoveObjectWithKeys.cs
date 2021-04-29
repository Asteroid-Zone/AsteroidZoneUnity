using Photon.Pun;
using Statics;
using UnityEngine;

namespace PlayGame.Player.Movement {
    
    /// <summary>
    /// This class is used for testing only. It allows the player to be controlled using the arrow keys.
    /// </summary>
    public class MoveObjectWithKeys : MonoBehaviourPun {

        private PlayerData _playerData;
        private GameObject _player;
        private MoveObject _moveObject;

        private void Start() {
            _player = gameObject;
            _playerData = GetComponent<PlayerData>();
            _moveObject = GetComponent<MoveObject>();
        }
        
        /// <summary>
        /// If the debug mode is enabled, this method allows the player to use the arrow keys to move and turn.
        /// <remarks>This method can only be called if this instance belongs to the local player.</remarks>
        /// </summary>
        private void Update() {
            if (!DebugSettings.Debug && !photonView.IsMine) return;
            if (!DebugSettings.ArrowKeys) return;
            
            // Move the target forward with the necessary speed smoothed by the delta time
            if (Input.GetKey(KeyCode.UpArrow)) _player.transform.Translate(Vector3.forward * (Time.deltaTime * _playerData.GetMaxSpeed()));

            // Move the target backward with the necessary speed smoothed by the delta time
            if (Input.GetKey(KeyCode.DownArrow)) _player.transform.Translate(Vector3.back * (Time.deltaTime * _playerData.GetMaxSpeed()));

            // Rotate the target to the left with the necessary rotational speed smoothed by the delta time
            if (Input.GetKey(KeyCode.LeftArrow)) {
                _moveObject.rotating = false;
                _player.transform.Rotate(Vector3.down, 50f * Time.deltaTime);
            }

            // Rotate the target to the right with the necessary rotational speed smoothed by the delta time
            if (Input.GetKey(KeyCode.RightArrow)) {
                _moveObject.rotating = false;
                _player.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
            }
        }
    }
}
