using PlayGame.Speech;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.VoiceChat {
    
    /// <summary>
    /// This class allows the player to switch between voice chat and command mode.
    /// </summary>
    public class VoiceChatSlider : MonoBehaviour {
        
        private Slider _voiceChatCommandSlider;
        
        private void Start() {
            // The initial mode in the game is Command mode, so voice chat should be muted
            VoiceChatController.MuteMyselfInVoiceChat();
            
            // Find the voice chat slider and set its on change listener
            _voiceChatCommandSlider = GetComponent<Slider>();
            _voiceChatCommandSlider.onValueChanged.AddListener(delegate
            {
                VoiceChatSliderChange();
            });
        }

        private void Update() {
            // The tab button changes the voice chat/command mode
            if (Input.GetKeyDown(KeyCode.Tab)) { // Switch voice chat and command modes
                _voiceChatCommandSlider.value = 1 - _voiceChatCommandSlider.value;
            }
        }

        /// <summary>
        /// This method switches the players mode depending on the value of the voice slider.
        /// </summary>
        private void VoiceChatSliderChange() {
            // Get the value of the voice chat slider
            float value = _voiceChatCommandSlider.value;
            
            switch (value) {
                case 0:
                    // Command mode: mute myself in voice chat and start speech recognition
                    VoiceChatController.MuteMyselfInVoiceChat();
                    SpeechRecognition.StartSpeechRecognitionInTheBrowser();
                    break;
                case 1:
                    // Voice chat mode: unmute myself in voice chat and start speech recognition
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
