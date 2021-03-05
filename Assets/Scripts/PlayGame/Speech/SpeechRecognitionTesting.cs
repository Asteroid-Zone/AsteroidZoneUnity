using UnityEngine;

namespace PlayGame.Speech
{
    public class SpeechRecognitionTesting : MonoBehaviour
    {

        public SpeechRecognition speechRecognition;

        void Update() {
            if (Input.GetKeyDown(KeyCode.W)) { // Move forward
                speechRecognition.GetResponse("move forward");
            }
            
            if (Input.GetKeyDown(KeyCode.S)) { // Move backwards
                speechRecognition.GetResponse("move back");
            }
            
            if (Input.GetKeyDown(KeyCode.A)) { // Start turning left
                speechRecognition.GetResponse("turn left");
            }
            
            if (Input.GetKeyDown(KeyCode.D)) { // Start turning right
                speechRecognition.GetResponse("turn right");
            }

            if (Input.GetKeyDown(KeyCode.Q)) { // Stop Moving and turning
                speechRecognition.GetResponse("stop");
            }

            if (Input.GetKeyDown(KeyCode.E)) { // Start Moving
                speechRecognition.GetResponse("go");
            }

            if (Input.GetKeyDown(KeyCode.R)) { // Move towards Grid(C, 6)
                speechRecognition.GetResponse("move to c6");
            }
        
            if (Input.GetKeyDown(KeyCode.F)) { // Move towards ping
                speechRecognition.GetResponse("go to ping");
            }
        
            if (Input.GetKeyDown(KeyCode.T)) { // Create ping at Grid(C, 4)
                speechRecognition.GetResponse("ping asteroid at C4");
            }
            
            if (Input.GetKeyDown(KeyCode.Y)) { // Create ping at Grid(G, 6)
                speechRecognition.GetResponse("ping pirate at G6");
            }
            
            if (Input.GetKeyDown(KeyCode.G)) { // Move towards the space station
                speechRecognition.GetResponse("go to the space station");
            }

            if (Input.GetKeyDown(KeyCode.H)) {
                speechRecognition.GetResponse("activate mining laser");
            }
            
            if (Input.GetKeyDown(KeyCode.J)) {
                speechRecognition.GetResponse("deactivate mining laser");
            }
            
            if (Input.GetKeyDown(KeyCode.Z)) {
                speechRecognition.GetResponse("transfer resources");
            }
            
            if (Input.GetKeyDown(KeyCode.X)) {
                speechRecognition.GetResponse("stop shooting");
            }

            if (Input.GetKeyDown(KeyCode.C)) {
                speechRecognition.GetResponse("shoot");
            }

            if (Input.GetKeyDown(KeyCode.L)) {
                speechRecognition.GetResponse("lock on nearest enemy");
            }
            
            if (Input.GetKeyDown(KeyCode.M)) {
                speechRecognition.GetResponse("disengage lock on");
            }
        }
    }
}
