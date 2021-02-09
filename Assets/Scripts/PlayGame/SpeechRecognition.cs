using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognition : MonoBehaviour
{

    public Text text;
    private string _myResponse = "...";

    public GameObject player;

    // Update is called once per frame
    private void Update()
    {
        text.text = _myResponse;
    }

    public void GetResponse(string result) {
        _myResponse = result.ToLower();
        performActions(_myResponse);
    }

    private void performActions(string phrase) {
        if (phrase.Contains("move") || phrase.Contains("face") || phrase.Contains("go")) {
            if (phrase.Contains("north")) {
                player.GetComponent<MoveObject>().setDirection(Vector3.forward);
            }

            if (phrase.Contains("south")) {
                player.GetComponent<MoveObject>().setDirection(Vector3.back);
            }

            if (phrase.Contains("west")) {
                player.GetComponent<MoveObject>().setDirection(Vector3.left);
            }

            if (phrase.Contains("east")) {
                player.GetComponent<MoveObject>().setDirection(Vector3.right);
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
