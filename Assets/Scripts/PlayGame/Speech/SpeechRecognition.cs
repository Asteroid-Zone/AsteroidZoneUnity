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

    private void Start()
    {
        _moveObject = player.GetComponent<MoveObject>();
    }

    private void Update()
    {
        text.text = _myResponse;
    }

    public void GetResponse(string result) {
        _myResponse = result.ToLower();
        PerformActions(_myResponse);
    }

    private bool IsMovementCommand(string phrase)
    {
        return phrase.Contains("move") || phrase.Contains("face") || phrase.Contains("go");
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
        Match coordMatch = Regex.Match(phrase, @"[a-z]( )?(\d+)"); // Letter Number with optional space
        
        if (!coordMatch.Success) return null;
        
        Match number = Regex.Match(coordMatch.Value, @"(\d+)");
        char x = coordMatch.Value[0];
        int y = int.Parse(number.Value);

        return new GridCoord(x, y);
    }

    private void PerformActions(string phrase) {
        if (IsMovementCommand(phrase)) {
            Vector3? direction = getDirection(phrase);
            if (direction != null) {
                _moveObject.SetDirection((Vector3) direction);
            } else {
                GridCoord? position = getGridPosition(phrase);
                if (position != null) {
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
