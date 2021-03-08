﻿using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace PlayGame.Camera
{
    public class TacticalCameraController : MonoBehaviour
    {
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

        private void Start()
        {
            _focusPoint = GameObject.Find("FocusPoint");
            _camera = GetComponent<UnityEngine.Camera>();
            transform.LookAt(_focusPoint.transform.position);
            CorrectCameraDistance();
        }

        private void Update()
        {
            var mouseDifference = Input.mousePosition - _prevMousePos;
            var camTransform = transform;

            if (Input.GetMouseButtonDown(0))
            {
                // Focus on an object if you clicked on it
                var ray = _camera.ScreenPointToRay( Input.mousePosition );

                if (Physics.Raycast(ray, out var hit, 1000f))
                {
                    // Start tracking the target
                    _trackingObject = hit.transform.gameObject;
                    _cameraDistance = minCamDistance;
                    FocusTrackedObject();
                }
            }
            else if (Input.GetMouseButton(1))
            {
                // Updates camera angle if RMB is held (based on mouse movement)
                transform.RotateAround(_focusPoint.transform.position, Vector3.up, mouseDifference.x * sensitivity);
            }
            else if (Input.GetMouseButton(2))
            {
                // Stop tracking
                _trackingObject = null;
                
                // Change camera position if MMB is held (based on mouse movement)
                var camForward = camTransform.forward;
                var camForwardXZ = new Vector3(camForward.x, 0, camForward.z);
                var positionChangeX = -mouseDifference.x * sensitivity * camTransform.right;
                var positionChangeY = -mouseDifference.y * sensitivity * camForwardXZ;
                var positionChange = positionChangeX + positionChangeY;
                var newFocusPosition = _focusPoint.transform.position + positionChange;
                if (_focusPointBounds.Contains(newFocusPosition))
                {
                    camTransform.position += positionChange;
                    _focusPoint.transform.position = newFocusPosition;
                }
            }
            else if (Input.mouseScrollDelta.y != 0)
            {
                // Zoom the camera in/out making sure it's within a certain distance from the focus point (min/maxCameraDistance)
                var newPosition = camTransform.position + (camTransform.forward * Input.mouseScrollDelta.y);
                var newCameraDistance = Mathf.Abs((newPosition - _focusPoint.transform.position).magnitude);
                if (minCamDistance <= newCameraDistance && newCameraDistance <= maxCamDistance)
                {
                    camTransform.position = newPosition;
                    _cameraDistance = newCameraDistance;
                }
            }
            
            if (_trackingObject) FocusTrackedObject();
            _prevMousePos = Input.mousePosition;
        }

        private void FocusTrackedObject()
        {
            var camTransform = transform;
            
            // Move the Focus Point game object to be centered on the target
            // TODO: See if this can be done nicer
            var focusPosition = _focusPoint.transform.position;
            var positionDifference = _trackingObject.transform.position - focusPosition;
            focusPosition += positionDifference;
            _focusPoint.transform.position = focusPosition;
            camTransform.position += positionDifference;
            
            CorrectCameraDistance();
        }

        private void CorrectCameraDistance()
        {
            // Reposition camera so that it's at the correct distance from the focus point
            var camTransform = transform;
            var focusPosition = _focusPoint.transform.position;
            camTransform.position = (camTransform.position - focusPosition).normalized * _cameraDistance + focusPosition;
        }
    }
}
