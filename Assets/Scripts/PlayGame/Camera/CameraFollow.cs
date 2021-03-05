using PhotonClass.GameController;
using PlayGame.Player.Movement;
using Statics;
using UnityEngine;

namespace PlayGame.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        private MoveObject _moveObject;

        // Distance from the target
        public float distance = 3.0f;

        // Height relative to the target
        public float height = 1.5f;
        public float damping = 5.0f;
        public bool smoothRotation = true;
        public bool followBehind = true;
        public float rotationDamping = 10.0f;

        public bool turn = true;

        private void Start() {
             if (!DebugSettings.Debug) target = PhotonPlayer.PP.myAvatar.transform;
             _moveObject = target.GetComponent<MoveObject>();
        }

        private void LateUpdate() {
            if (turn) {
                // Follow the player from behind
                var  wantedPosition = target.TransformPoint(0, height, followBehind ? -distance : distance);
                transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);
            
                if (smoothRotation)
                {
                    var wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
                }
                else transform.LookAt(target, target.up);
            } else {
                // Keep the camera orientation the same but follow the player
                float followDistance = followBehind ? -distance : distance;
                Vector3 wantedPosition = target.TransformPoint(0, height,  followDistance);
                transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);
            }
        }
    }
}

