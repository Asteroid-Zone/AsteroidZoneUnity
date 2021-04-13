using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class MenuFunctions : MonoBehaviour
    {
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
        
        public void ShowOrHideMenuPanel()
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
        }

        public void ShowTipsPanel()
        {
            if (quest != null) quest.SetActive(false);
            tipsPanel.SetActive(true);
        }

        public void HideTipsPanel()
        {
            if (quest != null) quest.SetActive(true);
            tipsPanel.SetActive(false);
        }

        public void ShowDebugPanel() {
            debugPanel.SetActive(true);
        }
        
        public void HideDebugPanel() {
            debugPanel.SetActive(false);
        }

        public void ToggleDebugMode() {
            DebugSettings.ArrowKeys = debugToggle.isOn;
            DebugSettings.DebugKeys = debugToggle.isOn;
        }

        public void ToggleSinglePlayer() {
            DebugSettings.SinglePlayer = singlePlayerToggle.isOn;
        }
        
    }
}
