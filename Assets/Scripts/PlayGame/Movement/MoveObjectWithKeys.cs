using UnityEngine;

namespace PlayGame.Movement {
    public class MoveObjectWithKeys : MonoBehaviour
    {

        private PlayerData _playerData;

        private void Start()
        {
            _playerData = GetComponent<PlayerData>();
        }
    
        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                // Move the target forward with the necessary speed smoothed by the delta time
                transform.Translate(Vector3.forward * (Time.deltaTime * _playerData.GetMaxSpeed()));
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                // Move the target backward with the necessary speed smoothed by the delta time
                transform.Translate(Vector3.back * (Time.deltaTime * _playerData.GetMaxSpeed()));
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                // Rotate the target to the left with the necessary rotational speed smoothed by the delta time
                transform.Rotate(Vector3.down, 50f * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                // Rotate the target to the right with the necessary rotational speed smoothed by the delta time
                transform.Rotate(Vector3.up, 50f * Time.deltaTime);
            }
        }
    }
}
