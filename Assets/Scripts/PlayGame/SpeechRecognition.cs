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
        performAction(_myResponse);
    }

    private void performAction(string phrase) {
        if (phrase.Contains("north")) {
            player.transform.Translate(Vector3.forward * (Time.deltaTime), Space.World);
        }

        if (phrase.Contains("south")) {
            player.transform.Translate(Vector3.back * (Time.deltaTime), Space.World);
        }

        if (phrase.Contains("west")) {
            player.transform.Translate(Vector3.left * (Time.deltaTime), Space.World);
        }

        if (phrase.Contains("east")) {
            player.transform.Translate(Vector3.right * (Time.deltaTime), Space.World);
        }
    }

    public void StartSpeechRecognitionInTheBrowser() {
        Application.ExternalCall("startButtonFromUnity3D");
    }

}
