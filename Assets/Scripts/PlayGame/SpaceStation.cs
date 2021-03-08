using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGame
{
    public class SpaceStation : MonoBehaviour {

        public GridManager gridManager;
        public UI.GameManager gameManager;

        private const int MaxStationHealth = 100;
        private int _stationHealth = 0;
        private int _stationShields = 100;
        private bool complete = false;
    
        private void Start() {
            transform.position = gridManager.GetGridCentre();
        }

        private void Update() {
            if (_stationHealth >= MaxStationHealth) {
                EventsManager.AddMessageToQueue("Game completed");
                // Ensures LeaveRoom is only called once
                if (!complete)
                {
                    gameManager.LeaveRoom(); // TODO create victory scene
                    complete = true;
                }
            }
        }

        public void AddResources(int resources) {
            _stationHealth += resources;
            EventsManager.AddMessageToQueue("Space station repaired");

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
