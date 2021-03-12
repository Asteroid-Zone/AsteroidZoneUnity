using PlayGame.SpaceStation;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI {
    public class DisplayStationStatus : MonoBehaviour {
    
        public GameObject spaceStation;

        private SpaceStation.SpaceStation _spaceStation;
        private Text _text;

        private void Start() {
            _text = GetComponent<Text>();
            _spaceStation = spaceStation.GetComponent<SpaceStation.SpaceStation>();
        }

        private void Update() {
            string text = "Station Modules\n";
            foreach (StationModule module in _spaceStation.GetModules()) {
                text += module + "\n";
            }
            _text.text = text;
        }
    }
}
