using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechRecognitionTesting : MonoBehaviour
{

    public SpeechRecognition speechRecognition;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            speechRecognition.GetResponse("move north");
        }
        if (Input.GetKey(KeyCode.S)) {
            speechRecognition.GetResponse("go south");
        }
        if (Input.GetKey(KeyCode.A)) {
            speechRecognition.GetResponse("west");
        }
        if (Input.GetKey(KeyCode.D)) {
            speechRecognition.GetResponse("east");
        }

        if (Input.GetKey(KeyCode.Q)) {
            speechRecognition.GetResponse("move north west");
        }
    }
}
