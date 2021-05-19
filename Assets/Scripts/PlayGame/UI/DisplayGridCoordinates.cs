using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayGame.UI {
    
    /// <summary>
    /// This class displays the target's current grid coordinates in the parent text GameObject.
    /// </summary>
    public class DisplayGridCoordinates : MonoBehaviour {
        
        public Transform target;
        private Text _text;
        
        private void Start() {
            target = (!DebugSettings.Debug && SceneManager.GetActiveScene().name != Scenes.TutorialScene) ? PhotonPlayer.Instance.myAvatar.transform : TestPlayer.GetPlayerShip().transform;
            _text = GetComponent<Text>();
        }

        private void Update() {
            if (target == null) return;
            
            // Get the coordinates of the target
            Vector3 coordinates = target.position;
            Vector2 gridCoords = GridManager.GlobalToGridCoord(coordinates);

            // Display the coordinates of the target rounded to 2 d.p.
            _text.text = $"({gridCoords.x}, {gridCoords.y})";
        }
    }
}