using UnityEngine;

namespace PlayGame.Speech
{
    public class SpeechRecognitionTesting : MonoBehaviour
    {

        public SpeechRecognition speechRecognition;

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
        
            if (Input.GetKey(KeyCode.F)) { // Move towards ping
                speechRecognition.GetResponse("go to ping");
            }
        
            if (Input.GetKey(KeyCode.T)) { // Create ping at Grid(C, 4)
                speechRecognition.GetResponse("ping asteroid at C4");
            }
            
            if (Input.GetKey(KeyCode.Y)) { // Create ping at Grid(C, 4)
                speechRecognition.GetResponse("ping pirate at C4");
            }
            
            if (Input.GetKey(KeyCode.G)) { // Move towards the space station
                speechRecognition.GetResponse("go to the space station");
            }

            if (Input.GetKey(KeyCode.H)) {
                speechRecognition.GetResponse("activate mining laser");
            }
            
            if (Input.GetKey(KeyCode.J)) {
                speechRecognition.GetResponse("deactivate mining laser");
            }
            
            if (Input.GetKey(KeyCode.Z)) {
                speechRecognition.GetResponse("transfer resources");
            }
            
            if (Input.GetKey(KeyCode.X)) {
                speechRecognition.GetResponse("stop shooting");
            }
            
            if (Input.GetKey(KeyCode.C)) {
                speechRecognition.GetResponse("shoot");
            }
        }
    }
}
