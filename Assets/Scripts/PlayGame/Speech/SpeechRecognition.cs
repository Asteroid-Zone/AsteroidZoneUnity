using Assets.Scripts.PlayGame;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognition : MonoBehaviour
{
    public Text text;
    private string _myResponse = "...";

    public GameObject player;
    private MoveObject _moveObject;

    private void Start() {
        _moveObject = player.GetComponent<MoveObject>();
    }

    private void Update() {
        text.text = _myResponse;
    }

    // Called by javascript when speech is detected
    public void GetResponse(string result) {
        _myResponse = result.ToLower();
        PerformActions(_myResponse);
    }

    private bool IsMovementCommand(string phrase) {
        return phrase.Contains("move") || phrase.Contains("face") || phrase.Contains("go");
    }

    // Returns the direction vector or null
    private Vector3? getDirection(string phrase) {
        if (phrase.Contains("north")) {
            return Vector3.forward;
        }

        if (phrase.Contains("south")) {
            return Vector3.back;
        }

        if (phrase.Contains("west")) {
            return Vector3.left;
        }

        if (phrase.Contains("east")) {
            return Vector3.right;
        }

        return null;
    }

    // Gets a grid coordinate from the input phrase, if no grid coordinate is found it returns null
    private GridCoord? getGridPosition(string phrase) {
        Match coordMatch = Regex.Match(phrase, @"[a-z]( )?(\d+)"); // Letter Number with optional space
        
        if (!coordMatch.Success) return null;
        
        Match number = Regex.Match(coordMatch.Value, @"(\d+)"); // One or more numbers
        char x = coordMatch.Value[0];
        int z = int.Parse(number.Value);

        return new GridCoord(x, z);
    }

    private void PerformActions(string phrase) {
        if (IsMovementCommand(phrase)) {
            Vector3? direction = getDirection(phrase);
            if (direction != null) { // If phrase contains a direction
                _moveObject.SetDirection((Vector3) direction);
            } else {
                GridCoord? position = getGridPosition(phrase);
                if (position != null) { // If phrase contains a grid coordinate
                    _moveObject.SetDirection((GridCoord) position);
                }
            }
        }

        if (phrase.Contains("stop")) {
            _moveObject.SetSpeed(0);
        }

        if (phrase.Contains("go") || phrase.Contains("move")) {
            _moveObject.SetSpeed(2.5f);
        }
    }

    public void StartSpeechRecognitionInTheBrowser() {
        Application.ExternalCall("startButtonFromUnity3D");
    }

}
