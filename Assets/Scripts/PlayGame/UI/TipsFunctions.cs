using UnityEngine;

namespace PlayGame.UI
{
    public class TipsFunctions : MonoBehaviour
    {
        public GameObject tipsPanel;

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
