﻿using UnityEngine;

public class MoveObjectWithKeys : MonoBehaviour
{
    // The speed of the moving object
    public float movementSpeed = 1;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // Move the target forward with the necessary speed smoothed by the delta time
            this.transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            // Move the target backward with the necessary speed smoothed by the delta time
            this.transform.Translate(Vector3.back * Time.deltaTime * movementSpeed);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // Rotate the target to the left with the necessary rotational speed smoothed by the delta time
            this.transform.Rotate(Vector3.down, 50f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            // Rotate the target to the right with the necessary rotational speed smoothed by the delta time
            this.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
        }
    }
}
