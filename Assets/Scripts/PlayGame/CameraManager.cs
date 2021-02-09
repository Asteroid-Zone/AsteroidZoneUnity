using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera followCamera, tacticalCamera;
    // private bool _camsSwitched = false;
    
    private void Start()
    {
        followCamera.enabled = true;
        tacticalCamera.enabled = false;
        // followCamera.gameObject.SetActive(true);
        // tacticalCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.P)) return;
        followCamera.enabled = !followCamera.enabled;
        tacticalCamera.enabled = !tacticalCamera.enabled;
        // _camsSwitched = !_camsSwitched;
        // followCamera.gameObject.SetActive(!_camsSwitched);
        // tacticalCamera.gameObject.SetActive(_camsSwitched);

    }
}
