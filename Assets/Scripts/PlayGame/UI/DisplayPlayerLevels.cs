﻿using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayGame.UI {
    
    /// <summary>
    /// This class displays the player's levels on the canvas.
    /// </summary>
    public class DisplayPlayerLevels : MonoBehaviour {
    
        public GameObject player;

        private PlayerData _playerData;
        public Text textCombat;
        public Text textMining;
        public Slider progressCombat;
        public Slider progressMining;

        private void Start() {
            player = (!DebugSettings.Debug && SceneManager.GetActiveScene().name != Scenes.TutorialScene) ? PhotonPlayer.Instance.myAvatar : TestPlayer.GetPlayerShip();
            _playerData = player.GetComponent<PlayerData>();
        }

        private void Update() {
            if (_playerData == null) return;
            
            textCombat.text = "Combat Level: " + _playerData.GetCombatLevel();
            textMining.text = "Mining Level: " + _playerData.GetMiningLevel();
            progressCombat.value = _playerData.GetCombatLevelProgress();
            progressMining.value = _playerData.GetMiningLevelProgress();
        }
    }
}