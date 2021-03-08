using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class VolumeControl : MonoBehaviour
    {
        public GameObject backgroundAudio;
        public Slider volumeSlider;
        private AudioSource _audioSource;
        
        private void Start()
        {
            _audioSource = backgroundAudio.GetComponent<AudioSource>();
            volumeSlider.value = _audioSource.volume;
            volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); });
        }

        private void ChangeVolume()
        {
            _audioSource.volume = volumeSlider.value;
        }
    }
}
