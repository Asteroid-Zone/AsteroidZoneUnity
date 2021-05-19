using PlayGame;
using PlayGame.Stats;
using UnityEngine;

namespace EndGame {
    
    /// <summary>
    /// This class controls the end game music.
    /// </summary>
    public class PostGameMusicSwitcher : MonoBehaviour {
        
        public AudioClip victoryMusic;
        public AudioClip defeatMusic;
        
        private AudioSource _audioSource;
        
        /// <summary>
        /// Plays the correct music for victory/defeat.
        /// </summary>
        private void Start() {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = StatsManager.GameStats.gameOverType == GameManager.GameOverType.Victory ? victoryMusic : defeatMusic;
            _audioSource.Play();
        }
    }
}
