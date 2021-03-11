using PlayGame.Stats;
using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class SpaceStation : MonoBehaviour {

        public GridManager gridManager;
        public GameManager gameManager;
        
        private int _stationShields = 100;
        private bool _complete = false;

        private StationModule _selectedModule;

        private Hyperdrive _hyperdrive;
    
        private void Start() {
            transform.position = gridManager.GetGridCentre();
            _hyperdrive = new Hyperdrive();
            _selectedModule = _hyperdrive;
        }

        private void Update() {
            if (_hyperdrive.isFunctional()) {
                // Ensures LeaveRoom is only called once
                if (!_complete) {
                    EventsManager.AddMessage("Game completed");
                    StatsManager.GameStats.endTime = Time.time;
                    gameManager.exitScene = Scenes.VictoryScene;
                    gameManager.LeaveRoom();
                    _complete = true;
                }
            }
        }

        public void AddResources(int resources) {
            Debug.Log("repairing");
            _selectedModule.Repair(resources);
            EventsManager.AddMessage(_selectedModule.name + " repaired (" + _selectedModule.moduleHealth + "/" + _selectedModule.maxHealth + ")");
        }

        public StationModule GetSelectedModule() {
            return _selectedModule;
        }

    }
}
