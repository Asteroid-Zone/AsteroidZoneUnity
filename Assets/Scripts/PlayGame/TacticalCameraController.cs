using UnityEngine;

public class TacticalCameraController : MonoBehaviour
{
    public float sensitivity = 0.25f;
    
    private Vector3 _prevMousePos = new Vector3(255f, 255f, 255f);
    private GameObject _focusPoint;

    private void Start()
    {
        _focusPoint = GameObject.Find("FocusPoint");
        transform.LookAt(_focusPoint.transform.position);
    }

    private void Update()
    {
        var mouseDifference = Input.mousePosition - _prevMousePos;
        var camTransform = transform;

        if (Input.GetMouseButton(1))
        {
            // Updates camera angle if RMB is held (based on mouse movement)
            transform.RotateAround(_focusPoint.transform.position, Vector3.up, mouseDifference.x * sensitivity);
            // var position = _focusPoint.transform.position;
            // transform.RotateAround(position, Vector3.up, 100f * Time.deltaTime * -1f);
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
