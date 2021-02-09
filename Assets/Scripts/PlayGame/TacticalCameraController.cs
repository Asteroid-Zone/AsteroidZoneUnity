using System.Collections;
using UnityEngine;

public class TacticalCameraController : MonoBehaviour
{
    public float sensitivity = 0.25f;
    
    private Vector3 _prevMousePos = new Vector3(255f, 255f, 255f);

    private void Update()
    {
        var mouseDifference = Input.mousePosition - _prevMousePos;
        var camTransform = transform;
        
        if (Input.GetMouseButton(1))
        {
            // Updates camera angle if RMB is held (based on mouse movement)
            transform.eulerAngles += new Vector3(-mouseDifference.y * sensitivity, mouseDifference.x * sensitivity, 0);
        } 
        else if (Input.GetMouseButton(2))
        {
            // Change camera position if MMB is held (based on mouse movement)
            camTransform.position += mouseDifference.x * sensitivity * camTransform.right;
        }
        else if (Input.mouseScrollDelta.y != 0)
        {
            // Zooms camera, a transform variable is used here to avoid repeated component property access (inefficient)
            camTransform.position += camTransform.forward * Input.mouseScrollDelta.y;
        }
        _prevMousePos = Input.mousePosition;
    }
}
