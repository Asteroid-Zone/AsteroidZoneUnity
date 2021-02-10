using UnityEngine;

public class SpeechRecognitionTesting : MonoBehaviour
{

    public SpeechRecognition speechRecognition;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) { // Face north and start moving
            speechRecognition.GetResponse("move north");
        }
        if (Input.GetKey(KeyCode.S)) { // Face south and start moving
            speechRecognition.GetResponse("go south");
        }
        if (Input.GetKey(KeyCode.A)) { // Face west but dont change speed
            speechRecognition.GetResponse("face west");
        }
        if (Input.GetKey(KeyCode.D)) { // Face east and start moving
            speechRecognition.GetResponse("move east");
        }

        if (Input.GetKey(KeyCode.Q)) { // Stop Moving
            speechRecognition.GetResponse("stop");
        }

        if (Input.GetKey(KeyCode.E)) { // Start Moving
            speechRecognition.GetResponse("go");
        }

        if (Input.GetKey(KeyCode.R)) { // Move towards Grid(C, 6)
            speechRecognition.GetResponse("go to C6");
        }
    }
}
