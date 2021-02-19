using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGame
{
    public class SpaceStation : MonoBehaviour {

        public GridManager gridManager;

        private const int MaxStationHealth = 100;
        private int _stationHealth = 0;
        private int _stationShields = 100;
    
        private void Start() {
            transform.position = gridManager.GetGridCentre();
        }

        private void Update() {
            if (_stationHealth >= MaxStationHealth) {
                SceneManager.LoadScene("MainMenu"); // TODO create victory scene
            }
        }

        public void AddResources(int resources) {
            _stationHealth += resources;

            if (_stationHealth > MaxStationHealth) _stationHealth = MaxStationHealth;
        }

        public int GetHealth()
        {
            return _stationHealth;
        }

        public int GetMaxHealth()
        {
            return MaxStationHealth;
        }

    }
}
