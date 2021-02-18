using UnityEngine;
using UnityEngine.UI;

namespace PlayGame.UI
{
    public class DisplayStationHealth : MonoBehaviour {
    
        public GameObject spaceStation;

        private SpaceStation _spaceStation;
        private Text _text;

        private void Start() {
            _text = GetComponent<Text>();
            _spaceStation = spaceStation.GetComponent<SpaceStation>();
        }

        private void Update()
        {
            _text.text = "Station Health: " + _spaceStation.GetHealth() + "/" + _spaceStation.GetMaxHealth();
        }
    }
}
