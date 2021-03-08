﻿using UnityEngine;

namespace PlayGame.UI
{
    public class MenuFunctions : MonoBehaviour
    {
        public GameObject menuPanel;
        public GameObject tipsPanel;
        
        public void ShowMenuPanel()
        {
            menuPanel.SetActive(true);
        }

        public void HideMenuPanel()
        {
            menuPanel.SetActive(false);
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