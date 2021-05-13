using Statics;
using UnityEngine;
using UnityEngine.Assertions;

namespace PlayGame.Speech {
    public class SpeechRecognitionTesting : MonoBehaviour {

        public SpeechRecognition speechRecognition;

        // Used letters:
        // Q W E R T Y U I O
        // A S D F G H J K L
        // Z X C V B N M
        private void Update() {
            if (!DebugSettings.DebugKeys) return;
            
            if (Input.GetKeyDown(KeyCode.O)) { // Move station to random location
                speechRecognition.GetFinalResponse("mine asteroids");
            }
            
            if (Input.GetKeyDown(KeyCode.N)) { // Respawn a random player
                speechRecognition.GetFinalResponse("respawn");
            }
            
            if (Input.GetKeyDown(KeyCode.W)) { // Move forward
                speechRecognition.GetFinalResponse("move forward");
            }
            
            if (Input.GetKeyDown(KeyCode.S)) { // Move backwards
                speechRecognition.GetFinalResponse("move back");
            }
            
            if (Input.GetKeyDown(KeyCode.A)) { // Start turning left
                speechRecognition.GetFinalResponse("turn left");
            }
            
            if (Input.GetKeyDown(KeyCode.D)) { // Start turning right
                speechRecognition.GetFinalResponse("turn right");
            }

            if (Input.GetKeyDown(KeyCode.U)) {
                speechRecognition.GetFinalResponse("move left");
            }
            
            if (Input.GetKeyDown(KeyCode.I)) {
                speechRecognition.GetFinalResponse("move right");
            }

            if (Input.GetKeyDown(KeyCode.Q)) { // Stop Moving and turning
                speechRecognition.GetFinalResponse("stop");
            }

            if (Input.GetKeyDown(KeyCode.E)) { // Start Moving
                speechRecognition.GetFinalResponse("go");
            }

            if (Input.GetKeyDown(KeyCode.R)) { // Move towards Grid(C, 3)
                speechRecognition.GetFinalResponse("move to c3");
            }
        
            if (Input.GetKeyDown(KeyCode.F)) { // Move towards ping
                speechRecognition.GetFinalResponse("go to ping");
            }
        
            if (Input.GetKeyDown(KeyCode.T)) { // Create ping at Grid(E, 6)
                speechRecognition.GetFinalResponse("ping asteroid at E6");
            }
            
            if (Input.GetKeyDown(KeyCode.Y)) { // Create ping at Grid(G, 6)
                speechRecognition.GetFinalResponse("ping pirate at G6");
            }
            
            if (Input.GetKeyDown(KeyCode.G)) { // Move towards the space station
                speechRecognition.GetFinalResponse("go to the space station");
            }

            if (Input.GetKeyDown(KeyCode.H)) {
                speechRecognition.GetFinalResponse("activate mining laser");
            }
            
            if (Input.GetKeyDown(KeyCode.J)) {
                speechRecognition.GetFinalResponse("deactivate mining laser");
            }
            
            if (Input.GetKeyDown(KeyCode.Z)) {
                speechRecognition.GetFinalResponse("transfer resources");
            }
            
            if (Input.GetKeyDown(KeyCode.X)) {
                speechRecognition.GetFinalResponse("stop shooting");
            }

            if (Input.GetKeyDown(KeyCode.C)) {
                speechRecognition.GetFinalResponse("shoot");
            }

            if (Input.GetKeyDown(KeyCode.L)) {
                speechRecognition.GetFinalResponse("lock on nearest enemy");
            }
            
            if (Input.GetKeyDown(KeyCode.K)) {
                speechRecognition.GetFinalResponse("target nearest asteroid");
            }
            
            if (Input.GetKeyDown(KeyCode.M)) {
                speechRecognition.GetFinalResponse("disengage lock on");
            }
            
            if (Input.GetKeyDown(KeyCode.B)) {
                speechRecognition.GetFinalResponse("activate hyperdrive");
            }
            
            if (Input.GetKeyDown(KeyCode.V)) {
                speechRecognition.GetFinalResponse("repair hyperdrive");
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                speechRecognition.GetFinalResponse("yes");
            }
        }

        private void Start() {
            // Grid Coord Assertions
            Assert.AreEqual(Grammar.GetGridCoord("a8"), "a8");
            Assert.AreEqual(Grammar.GetGridCoord("7b"), "b7");
            Assert.AreEqual(Grammar.GetGridCoord("a ate"), "a8");
            Assert.AreEqual(Grammar.GetGridCoord("be 6"), "b6");
            Assert.AreEqual(Grammar.GetGridCoord("bee free"), "b3");
            Assert.AreEqual(Grammar.GetGridCoord("eye 3"), "i3");
        }
        
    }
    
}
