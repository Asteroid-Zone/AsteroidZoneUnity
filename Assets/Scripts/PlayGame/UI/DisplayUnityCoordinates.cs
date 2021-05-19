using Photon.GameControllers;
using PlayGame.Player;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayGame.UI {
    
    /// <summary>
    /// This class displays the player's current unity coordinates in the parent text GameObject.
    /// </summary>
    public class DisplayUnityCoordinates : MonoBehaviour {
        
        public Transform target;
        private Text _text;

        private void Start() {
            target = (!DebugSettings.Debug && SceneManager.GetActiveScene().name != Scenes.TutorialScene) ? PhotonPlayer.Instance.myAvatar.transform : TestPlayer.GetPlayerShip().transform;
            _text = GetComponent<Text>();
        }

        private void Update() {
            if (target == null) return;
            
            var coordinates = target.position;

            // Display the coordinates of the target rounded to 2 d.p.
            _text.text = $"({coordinates.x:0.##}, {coordinates.y:0.##}, {coordinates.z:0.##})";
        }
    }
}
