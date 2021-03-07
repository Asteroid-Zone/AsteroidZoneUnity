using Photon.Pun;
using Statics;
using UnityEngine;

namespace PlayGame.Player.Movement {
    public class MoveObjectWithKeys : MonoBehaviourPun
    {

        private PlayerData _playerData;
        private GameObject _player;
        private MoveObject _moveObject;

        private void Start()
        {
            _player = gameObject;
            _playerData = GetComponent<PlayerData>();
            _moveObject = GetComponent<MoveObject>();
        }
        
        // Update is called once per frame
        private void Update()
        {
            if (!DebugSettings.Debug && !this.photonView.IsMine) return;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                // Move the target forward with the necessary speed smoothed by the delta time
                _player.transform.Translate(Vector3.forward * (Time.deltaTime * _playerData.GetMaxSpeed()));
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                // Move the target backward with the necessary speed smoothed by the delta time
                _player.transform.Translate(Vector3.back * (Time.deltaTime * _playerData.GetMaxSpeed()));
            }

            if (Input.GetKey(KeyCode.LeftArrow)) {
                _moveObject.rotating = false;
                // Rotate the target to the left with the necessary rotational speed smoothed by the delta time
                _player.transform.Rotate(Vector3.down, 50f * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow)) {
                _moveObject.rotating = false;
                // Rotate the target to the right with the necessary rotational speed smoothed by the delta time
                _player.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
            }
        }
    }
}
