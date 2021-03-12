using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class VolumeControl : MonoBehaviour
    {
        // Store the VolumePref Key to avoid typos
        private const string VolumePrefKey = "VolumePreference";
        
        public Slider volumeSlider;
        private AudioSource _audioSource;
        
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();

            // Get the volume from the prefs
            if (PlayerPrefs.HasKey(VolumePrefKey))
            {
                _audioSource.volume = PlayerPrefs.GetFloat(VolumePrefKey);
            }
            
            // Set the volume and register the listener to the slider changed event
            volumeSlider.value = _audioSource.volume;
            volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); });
        }

        private void ChangeVolume()
        {
            _audioSource.volume = volumeSlider.value;
            
            // Save the volume in the prefs
            PlayerPrefs.SetFloat(VolumePrefKey, volumeSlider.value);
        }
    }
}
