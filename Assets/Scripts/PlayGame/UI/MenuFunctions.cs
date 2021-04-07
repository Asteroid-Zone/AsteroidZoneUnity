using System.Xml.Serialization;
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

        public void Start() {
            debugToggle.isOn = DebugSettings.DebugKeys || DebugSettings.ArrowKeys;
        }
        
        public void ShowOrHideMenuPanel()
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
        }

        public void ShowTipsPanel()
        {
            tipsPanel.SetActive(true);
        }

        public void HideTipsPanel()
        {
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
        
    }
}
