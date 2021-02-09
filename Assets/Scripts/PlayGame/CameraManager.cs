using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera followCamera, tacticalCamera;

    private void Start()
    {
        followCamera.enabled = true;
        tacticalCamera.enabled = false;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.P)) return;
        followCamera.enabled = !followCamera.enabled;
        tacticalCamera.enabled = !tacticalCamera.enabled;
    }
}
