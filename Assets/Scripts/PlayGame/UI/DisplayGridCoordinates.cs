using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class DisplayGridCoordinates : MonoBehaviour
    {
        public Transform target;
        private Text _text;
        public int cellSize = 10;

        private void Start()
        {
            _text = GetComponent<Text>();
        }

        private void Update()
        {
            // Get the coordinates of the target
            var coordinates = target.position;

            // Display the coordinates of the target rounded to 2 d.p.
            _text.text = $"({(int)coordinates.x/cellSize % cellSize}, {(int)coordinates.z/cellSize % cellSize})";
        }
    }
}