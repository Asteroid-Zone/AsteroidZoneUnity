using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognition : MonoBehaviour
{

    public Text text;
    private string myResponse = "...";

    // Update is called once per frame
    void Update()
    {
        text.text = myResponse;
    }

    public void GetResponse(string result) {
        myResponse = result;
    }

    public void StartSpeechRecognitionInTheBrowser() {
        Application.ExternalCall("startButtonFromUnity3D");
    }

}
