using Assets.Scripts.PlayGame;
using System.Collections;
using System.Collections.Generic;
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

    private void performActions(string phrase) {
        if (isMovementCommand(phrase)) {
            Vector3? direction = getDirection(phrase);
            if (direction != null) {
                player.GetComponent<MoveObject>().setDirection((Vector3) direction);
            } else {

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
