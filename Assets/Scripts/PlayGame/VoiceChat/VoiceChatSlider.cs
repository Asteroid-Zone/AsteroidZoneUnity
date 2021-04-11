using PlayGame.Speech;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.VoiceChat
{
    public class VoiceChatSlider : MonoBehaviour
    {
        private Slider _voiceChatCommandSlider;
        
        private void Start()
        {
            _voiceChatCommandSlider = GetComponent<Slider>();
            _voiceChatCommandSlider.onValueChanged.AddListener(delegate
            {
                VoiceChatSliderChange();
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab)) { // Switch voice chat and command modes
                _voiceChatCommandSlider.value = 1 - _voiceChatCommandSlider.value;
            }
        }

        private void VoiceChatSliderChange()
        {
            float value = _voiceChatCommandSlider.value;
            
            switch (value)
            {
                case 0:
                    VoiceChatController.MuteMyselfInVoiceChat();
                    SpeechRecognition.StartSpeechRecognitionInTheBrowser();
                    break;
                case 1:
                    VoiceChatController.UnmuteMyselfInVoiceChat();
                    SpeechRecognition.StopSpeechRecognitionInTheBrowser();
                    break;
                default:
                    Debug.LogError($"Invalid value for VoiceChatSwitchSlider: {value}");
                    break;
            }
        }
    }
}
