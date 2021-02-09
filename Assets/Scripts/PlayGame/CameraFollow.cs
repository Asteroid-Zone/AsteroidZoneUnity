﻿using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    // Distance from the target
    public float distance = 3.0f;

    // Height relative to the target
    public float height = 1.5f;
    public float damping = 5.0f;
    public bool smoothRotation = true;
    public bool followBehind = true;
    public float rotationDamping = 10.0f;

    void LateUpdate()
    {
        Vector3 wantedPosition;
        if (followBehind)
        {
            wantedPosition = target.TransformPoint(0, height, -distance);
        }
        else
        {
            wantedPosition = target.TransformPoint(0, height, distance);
        }

        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);

        if (smoothRotation)
        {
            Quaternion wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
        }
        else transform.LookAt(target, target.up);
    }
}

