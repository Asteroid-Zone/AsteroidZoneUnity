using PhotonClass.GameController;
using UnityEngine;

namespace PlayGame.Player.Movement {
    public class MoveObjectWithKeys : MonoBehaviour
    {

        private PlayerData _playerData;
        public GameObject player;

        private void Start()
        {
            //player = PhotonPlayer.PP.myAvatar;
            _playerData = GetComponent<PlayerData>();
        }
    
        // Update is called once per frame
        private void Update()
        {
            //if (PhotonPlayer.PP.IsMine())
            //{
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    // Move the target forward with the necessary speed smoothed by the delta time
                    player.transform.Translate(Vector3.forward * (Time.deltaTime * _playerData.GetMaxSpeed()));
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    // Move the target backward with the necessary speed smoothed by the delta time
                    player.transform.Translate(Vector3.back * (Time.deltaTime * _playerData.GetMaxSpeed()));
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    // Rotate the target to the left with the necessary rotational speed smoothed by the delta time
                    player.transform.Rotate(Vector3.down, 50f * Time.deltaTime);
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    // Rotate the target to the right with the necessary rotational speed smoothed by the delta time
                    player.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
                }
            //}
        }
    }
}
