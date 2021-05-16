using PlayGame;
using PlayGame.Stats;
using UnityEngine;

namespace EndGame
{
    public class PostGameMusicSwitcher : MonoBehaviour
    {
        public AudioClip victoryMusic;
        public AudioClip defeatMusic;
        
        private AudioSource _audioSource;
        
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = StatsManager.GameStats.gameOverType == GameManager.GameOverType.Victory ? victoryMusic : defeatMusic;
            _audioSource.Play();
        }
    }
}
