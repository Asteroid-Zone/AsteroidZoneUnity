using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognition : MonoBehaviour
{

    public Text text;
    private string _myResponse = "...";

    // Update is called once per frame
    private void Update()
    {
        text.text = _myResponse;
    }

    public void GetResponse(string result) {
        _myResponse = result;
    }

    public void StartSpeechRecognitionInTheBrowser() {
        Application.ExternalCall("startButtonFromUnity3D");
    }

}
