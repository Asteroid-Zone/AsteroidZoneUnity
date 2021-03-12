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

        // todo select modules
        private StationModule _selectedModule;

        private readonly List<StationModule> _stationModules = new List<StationModule>();

        private Hyperdrive _hyperdrive;
        private ShieldGenerator _shieldGenerator;
        private StationHull _stationHull;
        // todo add turrets

        private void Start() {
            transform.position = gridManager.GetGridCentre();
            
            _hyperdrive = new Hyperdrive();
            _shieldGenerator = new ShieldGenerator();
            _stationHull = new StationHull(this);
            
            _stationModules.Add(_hyperdrive);
            _stationModules.Add(_shieldGenerator);
            _stationModules.Add(_stationHull);
            
            _selectedModule = _hyperdrive;
        }

        private void Update() {
            if (_hyperdrive.isFunctional()) {
                GameOver(true);
            }
            
            _shieldGenerator.Update();
        }

        public void GameOver(bool victory) {
            // Ensures LeaveRoom is only called once
            if (!_complete) {
                string eventMessage = "Game over";
                if (victory) eventMessage = "Game completed";
                EventsManager.AddMessage(eventMessage);
                StatsManager.GameStats.victory = victory;
                StatsManager.GameStats.endTime = Time.time;
                gameManager.exitScene = Scenes.VictoryScene;
                gameManager.LeaveRoom();
                _complete = true;
            }
        }

        public void AddResources(int resources) {
            _selectedModule.Repair(resources);
            EventsManager.AddMessage(_selectedModule.name + " repaired (" + _selectedModule.moduleHealth + "/" + _selectedModule.maxHealth + ")");
        }

        public void TakeDamage(int damage) {
            int damageRemaining = _shieldGenerator.AbsorbDamage(damage); // Shields take as much of the damage as they can

            // Random module takes a random amount of damage
            int moduleDamage = Random.Range(0, damageRemaining);
            _stationModules[Random.Range(0, _stationModules.Count)].TakeDamage(moduleDamage);
            damageRemaining -= moduleDamage;
            
            _stationHull.TakeDamage(damageRemaining); // Hull takes the rest of the damage
        }

        public StationModule GetStationHull() {
            return _stationHull;
        }

        public List<StationModule> GetModules() {
            return _stationModules;
        }
    }
}
