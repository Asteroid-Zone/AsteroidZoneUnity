using UnityEngine;

namespace PlayGame.UI
{
    public class MenuFunctions : MonoBehaviour
    {
        public GameObject menuPanel;
        public GameObject tipsPanel;
        
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
    }
}
