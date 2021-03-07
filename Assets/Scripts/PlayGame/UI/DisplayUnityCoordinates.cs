using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class DisplayUnityCoordinates : MonoBehaviour
    {
        public Transform target;
        private Text _text;

        private void Start()
        {
            target = !DebugSettings.Debug ? PhotonPlayer.Instance.myAvatar.transform : TestPlayer.GetPlayerShip().transform;
            _text = GetComponent<Text>();
        }

        private void Update()
        {
            // Get the coordinates of the target
            var coordinates = target.position;

            // Display the coordinates of the target rounded to 2 d.p.
            _text.text = $"({coordinates.x:0.##}, {coordinates.y:0.##}, {coordinates.z:0.##})";
        }
    }
}
