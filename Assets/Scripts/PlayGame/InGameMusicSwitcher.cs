using System.Collections;
using UnityEngine;

namespace PlayGame {
    
    /// <summary>
    /// This class controls the in-game music.
    /// </summary>
    public class InGameMusicSwitcher : MonoBehaviour {
        
        public AudioClip defaultMusic;
        public AudioClip combatMusic;

        private AudioSource _audioSource;
        private const float FadeDuration = 0.5f;

        private void Start() {
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Changes the music to the correct track.
        /// </summary>
        /// <param name="combat">True plays combat music, false plays normal music.</param>
        public void SetMusic(bool combat) {
            StartCoroutine(FadeAudio(combat));
        }

        /// <summary>
        /// Fades from one music track to the other.
        /// </summary>
        /// <param name="combat">True plays combat music, false plays normal music.</param>
        private IEnumerator FadeAudio(bool combat) {
            var startVolume = _audioSource.volume;
            var targetClip = combat ? combatMusic : defaultMusic;
            var currentTime = 0f;
            
            while (currentTime < FadeDuration) {
                currentTime += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(startVolume, 0, currentTime / FadeDuration);
                yield return null;
            }

            _audioSource.clip = targetClip;
            _audioSource.Play();
            
            currentTime = 0f;
            while (currentTime < FadeDuration) {
                currentTime += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(0, startVolume, currentTime / FadeDuration);
                yield return null;
            }
        }
    }
}
