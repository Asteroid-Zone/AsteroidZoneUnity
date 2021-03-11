using PlayGame.SpaceStation;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class DisplayStationHealth : MonoBehaviour {
    
        public GameObject spaceStation;

        private SpaceStation.SpaceStation _spaceStation;
        private Text _text;

        private void Start() {
            _text = GetComponent<Text>();
            _spaceStation = spaceStation.GetComponent<SpaceStation.SpaceStation>();
        }

        private void Update() {
            StationModule module = _spaceStation.GetSelectedModule();
            _text.text = module.name + " Health: " + module.moduleHealth + "/" + module.maxHealth;
        }
    }
}
