using Statics;
using UnityEngine;

namespace PlayGame.Player
{
    public class TestPlayer : MonoBehaviour
    {
        // Enable the test player ship if Debug mode.
        private void Start()
        {
            if (!DebugSettings.Debug)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
