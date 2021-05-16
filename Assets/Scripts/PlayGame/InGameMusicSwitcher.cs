using System.Collections;
using UnityEngine;

namespace PlayGame
{
    public class InGameMusicSwitcher : MonoBehaviour
    {
        public AudioClip defaultMusic;
        public AudioClip combatMusic;

        private AudioSource _audioSource;
        private const float FadeDuration = 0.5f;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void SetMusic(bool combat)
        {
            StartCoroutine(FadeAudio(combat));
        }

        private IEnumerator FadeAudio(bool combat)
        {
            var startVolume = _audioSource.volume;
            var targetClip = combat ? combatMusic : defaultMusic;
            var currentTime = 0f;
            while (currentTime < FadeDuration)
            {
                currentTime += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(startVolume, 0, currentTime / FadeDuration);
                yield return null;
            }

            _audioSource.clip = targetClip;
            _audioSource.Play();
            
            currentTime = 0f;
            while (currentTime < FadeDuration)
            {
                currentTime += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(0, startVolume, currentTime / FadeDuration);
                yield return null;
            }
        }
    }
}
