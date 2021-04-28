using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;

namespace PlayGame.Camera {
    
    /// <summary>
    /// This class controls the camera following the player.
    /// </summary>
    public class CameraFollow : MonoBehaviour {
        
        public Transform target;
        
        public float distance = 3.0f; // Distance from the target
        public float height = 1.5f; // Height relative to the target
        
        public float damping = 5.0f;
        public bool smoothRotation = true;
        public bool followBehind = true;
        public float rotationDamping = 10.0f;

        private void Start() {
            // Sets the target to be the player
            target = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar.transform : TestPlayer.GetPlayerShip().transform;
        }

        /// <summary>
        /// <para>LateUpdate is called every frame, if the Behaviour is enabled.</para>
        /// Move and rotate the camera to follow the player.
        /// </summary>
        private void LateUpdate() {
            if (target == null) return;
            
            // Calculate the wanted position of the camera
            var  wantedPosition = target.TransformPoint(0, height, followBehind ? -distance : distance);
            // Linearly interpolate between the current and wanted position of the camera
            transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);
            
            // If smooth rotation is enabled calculate the wanted rotation then spherically interpolate, otherwise rotate to look at the player
            if (smoothRotation) {
                var wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
            }
            else transform.LookAt(target, target.up);
        }
    }
}