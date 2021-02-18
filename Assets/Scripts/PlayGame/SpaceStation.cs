using UnityEngine;

namespace PlayGame
{
    public class SpaceStation : MonoBehaviour {

        public GridManager gridManager;

        private const int MaxStationHealth = 100;
        private int _stationHealth = 0;
        private int _stationShields = 100;
    
        void Start() {
            transform.position = gridManager.GetGridCentre();
        }

        public void AddResources(int resources)
        {
            _stationHealth += resources;
        }

    }
}
