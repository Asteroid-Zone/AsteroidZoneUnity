using UnityEngine;

public class TacticalCameraController : MonoBehaviour
{
    public float sensitivity = 0.25f;
    
    private Vector3 _prevMousePos = new Vector3(255f, 255f, 255f);
    private GameObject _focusPoint;
    private Camera _camera;

    private void Start()
    {
        _focusPoint = GameObject.Find("FocusPoint");
        _camera = GetComponent<Camera>();
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
                // TODO: See if this can be done nicer
                var position = _focusPoint.transform.position;
                var positionDifference = hit.transform.position - position;
                position += positionDifference;
                _focusPoint.transform.position = position;
                transform.position += positionDifference;
                transform.LookAt(position);
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
            var positionChange = mouseDifference.x * sensitivity * camTransform.right;
            camTransform.position += positionChange;
            _focusPoint.transform.position += positionChange;
        }
        else if (Input.mouseScrollDelta.y != 0)
        {
            // Zooms camera, a transform variable is used here to avoid repeated component property access (inefficient)
            camTransform.position += camTransform.forward * Input.mouseScrollDelta.y;
        }
        _prevMousePos = Input.mousePosition;
    }
}
