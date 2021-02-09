using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSky : MonoBehaviour
{
    public float rotateSpeed= 1.2f;
    private static readonly int Rotation = Shader.PropertyToID("_Rotation");

    // Update is called once per frame
    private void Update()
    {
        RenderSettings.skybox.SetFloat(Rotation, Time.time * rotateSpeed);
    }
}
