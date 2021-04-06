using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class MenuFunctions : MonoBehaviour
    {
        public GameObject menuPanel;
        public GameObject tipsPanel;
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

        public void ToggleDebugMode() {
            DebugSettings.ArrowKeys = debugToggle.isOn;
            DebugSettings.DebugKeys = debugToggle.isOn;
        }
        
    }
}
