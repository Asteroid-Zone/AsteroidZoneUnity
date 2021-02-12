﻿using UnityEngine;

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

        private void Start()
        {
            _focusPoint = GameObject.Find("FocusPoint");
            _camera = GetComponent<UnityEngine.Camera>();
            transform.LookAt(_focusPoint.transform.position);
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
                    // Move the Focus Point game object to be centered on the target
                    // TODO: See if this can be done nicer
                    var focusPosition = _focusPoint.transform.position;
                    var positionDifference = hit.transform.position - focusPosition;
                    focusPosition += positionDifference;
                    _focusPoint.transform.position = focusPosition;
                
                    // Reposition the camera so that it's at the minimum distance from the new focus point position
                    camTransform.position = (camTransform.position - focusPosition + positionDifference).normalized * minCamDistance + focusPosition;
                }
            }
            else if (Input.GetMouseButton(1))
            {
                // Updates camera angle if RMB is held (based on mouse movement)
                transform.RotateAround(_focusPoint.transform.position, Vector3.up, mouseDifference.x * sensitivity);
            }
            else if (Input.GetMouseButton(2))
            {
                // Change camera position if MMB is held (based on mouse movement)
                var positionChange = -mouseDifference.x * sensitivity * camTransform.right;
                camTransform.position += positionChange;
                _focusPoint.transform.position += positionChange;
            }
            else if (Input.mouseScrollDelta.y != 0)
            {
                // Zoom the camera in/out making sure it's within a certain distance from the focus point (min/maxCameraDistance)
                var newPosition = camTransform.position + (camTransform.forward * Input.mouseScrollDelta.y);
                var newCameraDistance = Mathf.Abs((newPosition - _focusPoint.transform.position).magnitude);
                if (minCamDistance <= newCameraDistance && newCameraDistance <= maxCamDistance)
                {
                    camTransform.position = newPosition;
                }
            }
            _prevMousePos = Input.mousePosition;
        }
    }
}