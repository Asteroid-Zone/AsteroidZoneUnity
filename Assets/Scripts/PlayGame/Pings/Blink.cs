using UnityEngine;

namespace PlayGame.Pings
{
    public class Blink : MonoBehaviour
    {
        private Renderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            _renderer.enabled = !(Time.fixedTime % 1f < 0.5f);
        }
    }
}
