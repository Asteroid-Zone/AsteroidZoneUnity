using System.Collections.Generic;
using Statics;
using Tutorial;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class VolumeControl : MonoBehaviour
    {
        // Make the class a singleton
        #region Singleton
        private static VolumeControl _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            } else {
                _instance = this;
            }
            
            // Add the default value for the background volume
            if (!PlayerPrefs.HasKey(BackVolumePrefKey))
            {
                PlayerPrefs.SetFloat(BackVolumePrefKey, BackVolumeDefault);
            }
            
            // Add the default value for the SFX volume
            if (!PlayerPrefs.HasKey(SfxVolumePrefKey))
            {
                PlayerPrefs.SetFloat(SfxVolumePrefKey, SfxVolumeDefault);
            }
            
            _sfxSources = new List<AudioSource>();
        }
        #endregion
        
        // Store the VolumePref Keys to avoid typos
        private const string BackVolumePrefKey = "BackgroundVolumePreference";
        private const string SfxVolumePrefKey = "SfxVolumePreference";

        // Default values for the different volumes
        private const float BackVolumeDefault = 0.1f;
        private const float SfxVolumeDefault = 0.1f;
        
        // Volume sliders
        public Slider backVolumeSlider;
        public Slider sfxVolumeSlider;
        
        // Background audio source
        private AudioSource _backAudioSource;

        // List of all the SFX audio sources
        private List<AudioSource> _sfxSources;
        
        private void Start()
        {
            // Get the background audio source
            if (SceneManager.GetActiveScene().name == Scenes.TutorialScene)
            {
                // Get the tutorial voice
                _backAudioSource = TutorialManager.GetInstance().gameObject
                    .GetComponent<AudioSource>();

                // Stop the background music
                GetComponent<AudioSource>().mute = true;
            }
            else
            {
                _backAudioSource = GetComponent<AudioSource>();
            }

            // Set the background volume (if tutorial- set initially to 1 to be as loud as possible)
            backVolumeSlider.value =_backAudioSource.volume = (SceneManager.GetActiveScene().name == Scenes.TutorialScene) ?
                1:
                PlayerPrefs.GetFloat(BackVolumePrefKey);


            // Set the value for the sfx volume slider and set the value for all SFX audio sources
            sfxVolumeSlider.value = PlayerPrefs.GetFloat(SfxVolumePrefKey);
            ChangeSfxVolume();
            
            // Register the listeners to the slider changed events
            backVolumeSlider.onValueChanged.AddListener(delegate { ChangeBackVolume(); });
            sfxVolumeSlider.onValueChanged.AddListener(delegate { ChangeSfxVolume(); });
        }

        private void ChangeBackVolume()
        {
            _backAudioSource.volume = backVolumeSlider.value;
            
            // Save the volume in the prefs
            PlayerPrefs.SetFloat(BackVolumePrefKey, backVolumeSlider.value);
        }

        private void ChangeSfxVolume()
        {
            // Set the new value in the prefs and set the volume of each source
            PlayerPrefs.SetFloat(SfxVolumePrefKey, sfxVolumeSlider.value);
            _sfxSources.ForEach(source => source.volume = sfxVolumeSlider.value);
        }

        public static void AddSfxCSource(AudioSource sfxSource)
        {
            if (sfxSource == null)
            {
                return;
            }
            
            // Add the source to the list and set its volume
            _instance._sfxSources.Add(sfxSource);
            sfxSource.volume = PlayerPrefs.GetFloat(SfxVolumePrefKey);
        }
    }
}
