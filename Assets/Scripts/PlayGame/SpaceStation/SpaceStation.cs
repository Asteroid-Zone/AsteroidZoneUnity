using System.Collections.Generic;
using PlayGame.Stats;
using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class SpaceStation : MonoBehaviour {

        public GridManager gridManager;
        public GameManager gameManager;
        
        private bool _complete = false;

        private StationModule _selectedModule;

        private Hyperdrive _hyperdrive;
        private ShieldGenerator _shieldGenerator;
    
        private void Start() {
            transform.position = gridManager.GetGridCentre();
            
            _hyperdrive = new Hyperdrive();
            _shieldGenerator = new ShieldGenerator();
            
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
            
            _shieldGenerator.Update();
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
