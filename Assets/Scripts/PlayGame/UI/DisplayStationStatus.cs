using PlayGame.SpaceStation;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI {
    
    /// <summary>
    /// This class displays the station's health for each module in the parent text GameObject.
    /// </summary>
    public class DisplayStationStatus : MonoBehaviour {
    
        public GameObject spaceStation;

        private SpaceStation.SpaceStation _spaceStation;
        private Text _text;

        private void Start() {
            _text = GetComponent<Text>();
            _spaceStation = spaceStation.GetComponent<SpaceStation.SpaceStation>();
        }

        /// <summary>
        /// Displays the health of each station module.
        /// </summary>
        private void Update() {
            if (_spaceStation == null) return;
            
            string text = "Station Resources: " + _spaceStation.resources + "\n";

            text += "Station Modules\n";
            foreach (StationModule module in _spaceStation.GetModules()) {
                text += module + "\n";
            }
            
            _text.text = text;
        }
    }
}
