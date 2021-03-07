using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class DisplayGridCoordinates : MonoBehaviour
    {
        public Transform target;
        public GridManager gridManager;
        private Text _text;
        private int _cellSize;
        
        private void Start()
        {
            target = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar.transform : TestPlayer.GetPlayerShip().transform;
            _text = GetComponent<Text>();
            _cellSize = gridManager.GetCellSize();
        }

        private void Update()
        {
            // Get the coordinates of the target
            var coordinates = target.position;

            // Display the coordinates of the target rounded to 2 d.p.
            _text.text = $"({(int)coordinates.x/_cellSize % _cellSize}, {(int)coordinates.z/_cellSize % _cellSize})";
        }
    }
}