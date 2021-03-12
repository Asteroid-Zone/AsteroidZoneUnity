﻿using System;
using System.Globalization;
using Photon.Pun;
using PlayGame.Stats;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EndGame {
    public class VictoryMenu : MonoBehaviour {
    
        public AudioSource buttonPress;

        public Text winText;

        public Text playerName;
        public Text playerResourcesHarvested;
        public Text playerAsteroidsDestroyed;
        public Text playerPiratesDestroyed;

        public Text gameTime;
        public Text gameResourcesHarvested;
        public Text gameAsteroidsDestroyed;
        public Text gamePiratesDestroyed;
        
        private PlayerStats _playerStats;
        
        private void Start() {
            winText.text = StatsManager.GameStats.victory ? "You Win!" : "You lose!";
            
            _playerStats = StatsManager.GetPlayerStats(PhotonNetwork.NickName);
            playerName.text += _playerStats.playerName;
            playerResourcesHarvested.text += _playerStats.resourcesHarvested;
            playerAsteroidsDestroyed.text += _playerStats.asteroidsDestroyed;
            playerPiratesDestroyed.text += _playerStats.piratesDestroyed;

            gameTime.text += FormatTime(StatsManager.GameStats.endTime - StatsManager.GameStats.startTime);
            gameResourcesHarvested.text += StatsManager.GameStats.resourcesHarvested;
            gameAsteroidsDestroyed.text += StatsManager.GameStats.asteroidsDestroyed;
            gamePiratesDestroyed.text += StatsManager.GameStats.piratesDestroyed;
        }

        private static string FormatTime(float seconds) {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            return ts.ToString(@"mm\:ss\:fff");
        }
        
        public void BackToMenu() {
            buttonPress.Play();
            SceneManager.LoadScene(Scenes.MainMenuScene);
        }
    }
}
