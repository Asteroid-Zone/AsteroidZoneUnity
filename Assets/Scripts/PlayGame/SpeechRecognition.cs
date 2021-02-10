using Assets.Scripts.PlayGame;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognition : MonoBehaviour
{

    public Text text;
    private string _myResponse = "...";

    public GameObject player;

    private void Update()
    {
        text.text = _myResponse;
    }

    public void GetResponse(string result) {
        _myResponse = result.ToLower();
        performActions(_myResponse);
    }

    private bool isMovementCommand(string phrase) {
        if (phrase.Contains("move") || phrase.Contains("face") || phrase.Contains("go")) {
            return true;
        }

        return false;
    }

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

    private GridCoord? getGridPosition(string phrase) {
        string pattern = @"(\d+)( )?[a-z]( )?(\d+)"; // Number Letter Number with optional spaces
        Match coordMatch = Regex.Match(phrase, pattern);
        if (coordMatch.Success) {
            Match numbers = Regex.Match(coordMatch.Value, @"(\d+)");
            Match letter = Regex.Match(coordMatch.Value, @"[a-z]");
            int x = int.Parse(numbers.Value);
            char y = letter.Value[0];
            numbers = numbers.NextMatch();
            int z = int.Parse(numbers.Value);

            return new GridCoord(x, y, z);
        }

        return null;
    }

    private void performActions(string phrase) {
        if (isMovementCommand(phrase)) {
            Vector3? direction = getDirection(phrase);
            if (direction != null) {
                player.GetComponent<MoveObject>().setDirection((Vector3) direction);
            } else {
                GridCoord? position = getGridPosition(phrase);
                if (position != null) {
                    player.GetComponent<MoveObject>().setDirection((GridCoord) position);
                }
            }
        }

        if (phrase.Contains("stop")) {
            player.GetComponent<MoveObject>().setSpeed(0);
        }

        if (phrase.Contains("go") || phrase.Contains("move")) {
            player.GetComponent<MoveObject>().setSpeed(1);
        }
    }

    public void StartSpeechRecognitionInTheBrowser() {
        Application.ExternalCall("startButtonFromUnity3D");
    }

}
