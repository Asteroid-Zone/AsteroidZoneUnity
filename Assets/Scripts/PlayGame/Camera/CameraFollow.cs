using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private const float MinCamDistance = 2f; // Minimum distance between the camera and its target
        private const float MaxCamDistance = 8f; // Maximum distance between the camera and its target

        private void Start() {
            // Sets the target to be the player
            target = (!DebugSettings.Debug && SceneManager.GetActiveScene().name != Scenes.TutorialScene) ? PhotonPlayer.Instance.myAvatar.transform : TestPlayer.GetPlayerShip().transform;
        }
        
        /// <summary>
        /// <para>LateUpdate is called every frame, if the Behaviour is enabled.</para>
        /// Move and rotate the camera to follow the player.
        /// </summary>
        private void LateUpdate() {
            if (target == null) return;
            
            // Check whether the camera should zoom in/out
            if (Input.mouseScrollDelta.y != 0) ZoomCamera(transform);
            
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
        
        /// <summary>
        /// <para>Method is called if the scroll wheel is used.</para>
        /// Updates the camera zoom based on mouse scrolling.
        /// </summary>
        /// <param name="camTransform"></param>
        private void ZoomCamera(Transform camTransform) {
            var newPosition = camTransform.position + (camTransform.forward * Input.mouseScrollDelta.y / 2);
            var newCameraDistance = Mathf.Abs((newPosition - target.position).magnitude);
            // Check the camera is within the min/max distances
            if (MinCamDistance <= newCameraDistance && newCameraDistance <= MaxCamDistance) {
                camTransform.position = newPosition;
                distance = newCameraDistance;
            }
        }
    }
}