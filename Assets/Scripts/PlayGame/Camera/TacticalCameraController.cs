using Statics;
using Tutorial;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace PlayGame.Camera {
    
    /// <summary>
    /// This class controls the tactical camera.
    /// </summary>
    public class TacticalCameraController : MonoBehaviour {
        
        public float sensitivity = 0.25f;
        public float minCamDistance = 10f;
        public float maxCamDistance = 100f;
    
        private Vector3 _prevMousePos = new Vector3(255f, 255f, 255f);
        private GameObject _focusPoint;
        private UnityEngine.Camera _camera;
        private float _cameraDistance = 50f;
        private GameObject _trackingObject;
        
        // TODO: Base this on the grid manager settings
        private Bounds _focusPointBounds = new Bounds(new Vector3(50f, 0f, 50f), new Vector3(100f, 1f, 100f));

        // Initially focus on the space station
        private void Start() {
            _focusPoint = GameObject.Find("FocusPoint");
            _trackingObject = GameObject.FindGameObjectWithTag(Tags.StationTag);
            FocusTrackedObject();
            
            _camera = GetComponent<UnityEngine.Camera>();
            transform.LookAt(_focusPoint.transform.position);
            CorrectCameraDistance();
        }
        
        // Perform actions based on mouse controls
        private void Update() {
            Vector3 mouseDifference = Input.mousePosition - _prevMousePos;
            Transform camTransform = transform;
            
            if (Input.GetMouseButtonDown(0)) {
                TutorialManager.Track = true;
                TrackObject();
            } else if (Input.GetMouseButton(1)) {
                TutorialManager.Rotate = true;
                RotateCamera(mouseDifference);
            } else if (Input.GetMouseButton(2)) {
                TutorialManager.Move = true;
                MoveCamera(mouseDifference, camTransform);
            } else if (Input.mouseScrollDelta.y != 0) {
                TutorialManager.Zoom = true;
                ZoomCamera(camTransform);
            }
            
            if (_trackingObject) FocusTrackedObject();
            _prevMousePos = Input.mousePosition;
        }
        
        /// <summary>
        /// <para>Method is called if the left mouse button is pressed.</para>
        /// If an object is clicked start tracking it.
        /// </summary>
        private void TrackObject() {
            // Focus on an object if you clicked on it
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 1000f)) {
                // Start tracking the target
                _trackingObject = hit.transform.gameObject;
                _cameraDistance = minCamDistance;
                FocusTrackedObject();
            }
        }

        /// <summary>
        /// <para>Method is called if the right mouse button is held.</para>
        /// Updates the camera angle based on mouse movement.
        /// </summary>
        /// <param name="mouseDifference"></param>
        private void RotateCamera(Vector3 mouseDifference) {
            transform.RotateAround(_focusPoint.transform.position, Vector3.up, mouseDifference.x * sensitivity);
        }

        /// <summary>
        /// <para>Method is called if the middle mouse button is held.</para>
        /// Updates the camera position based on mouse movement.
        /// </summary>
        /// <param name="mouseDifference"></param>
        /// <param name="camTransform"></param>
        private void MoveCamera(Vector3 mouseDifference, Transform camTransform) {
            _trackingObject = null; // Stop tracking
                
            var camForward = camTransform.forward;
            var camForwardXZ = new Vector3(camForward.x, 0, camForward.z);
            var positionChangeX = -mouseDifference.x * sensitivity * camTransform.right;
            var positionChangeY = -mouseDifference.y * sensitivity * camForwardXZ;
            var positionChange = positionChangeX + positionChangeY;
            var newFocusPosition = _focusPoint.transform.position + positionChange;
            if (_focusPointBounds.Contains(newFocusPosition)) {
                camTransform.position += positionChange;
                _focusPoint.transform.position = newFocusPosition;
            }
        }

        /// <summary>
        /// <para>Method is called if the scroll wheel is used.</para>
        /// Updates the camera zoom based on mouse scrolling.
        /// </summary>
        /// <param name="camTransform"></param>
        private void ZoomCamera(Transform camTransform) {
            var newPosition = camTransform.position + (camTransform.forward * Input.mouseScrollDelta.y);
            var newCameraDistance = Mathf.Abs((newPosition - _focusPoint.transform.position).magnitude);
            // Check the camera is within the min/max distances
            if (minCamDistance <= newCameraDistance && newCameraDistance <= maxCamDistance) {
                camTransform.position = newPosition;
                _cameraDistance = newCameraDistance;
            }
        }

        /// <summary>
        /// Updates the camera position to follow the tracked object.
        /// </summary>
        private void FocusTrackedObject() {
            var camTransform = transform;
            
            // TODO: See if this can be done nicer
            // Move the Focus Point game object to be centered on the target
            var focusPosition = _focusPoint.transform.position;
            var positionDifference = _trackingObject.transform.position - focusPosition;
            focusPosition += positionDifference;
            _focusPoint.transform.position = focusPosition;
            camTransform.position += positionDifference;
            
            CorrectCameraDistance();
        }

        /// <summary>
        /// Repositions the camera so that it's at the correct distance from the focus point
        /// </summary>
        private void CorrectCameraDistance() {
            var camTransform = transform;
            var focusPosition = _focusPoint.transform.position;
            camTransform.position = (camTransform.position - focusPosition).normalized * _cameraDistance + focusPosition;
        }
    }
}
