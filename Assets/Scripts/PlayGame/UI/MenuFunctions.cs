using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI {
    
    /// <summary>
    /// This class controls the in-game menu panel.
    /// </summary>
    public class MenuFunctions : MonoBehaviour {
        
        public GameObject menuPanel;
        public GameObject tipsPanel;
        public GameObject debugPanel;
        public Toggle debugToggle;
        public Toggle singlePlayerToggle;
        public GameObject quest;

        public void Start() {
            debugToggle.isOn = DebugSettings.DebugKeys || DebugSettings.ArrowKeys;
            if (singlePlayerToggle != null) singlePlayerToggle.isOn = DebugSettings.SinglePlayer;
        }
        
        /// <summary>
        /// Toggles the menu panel.
        /// </summary>
        public void ShowOrHideMenuPanel() {
            menuPanel.SetActive(!menuPanel.activeSelf);
        }

        /// <summary>
        /// Shows the tips panel.
        /// </summary>
        public void ShowTipsPanel() {
            if (quest != null) quest.SetActive(false);
            tipsPanel.SetActive(true);
        }

        /// <summary>
        /// Hides the tips panel.
        /// </summary>
        public void HideTipsPanel() {
            if (quest != null) quest.SetActive(true);
            tipsPanel.SetActive(false);
        }

        /// <summary>
        /// Shows the debug settings panel.
        /// </summary>
        public void ShowDebugPanel() {
            debugPanel.SetActive(true);
        }
        
        /// <summary>
        /// Hides the debug settings panel.
        /// </summary>
        public void HideDebugPanel() {
            debugPanel.SetActive(false);
        }

        /// <summary>
        /// Toggles the keyboard controls.
        /// </summary>
        public void ToggleDebugMode() {
            DebugSettings.ArrowKeys = debugToggle.isOn;
            DebugSettings.DebugKeys = debugToggle.isOn;
        }

        /// <summary>
        /// Toggles the single player mode.
        /// </summary>
        public void ToggleSinglePlayer() {
            DebugSettings.SinglePlayer = singlePlayerToggle.isOn;
        }
        
    }
}
