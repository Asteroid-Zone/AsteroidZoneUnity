using System;
using System.Collections.Generic;
using PlayGame.Speech.Commands;
using PlayGame.Stats;
using PlayGame.UI;
using Statics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayGame.SpaceStation {
    public class SpaceStation : MonoBehaviour {

        public GridManager gridManager;
        public GameManager gameManager;
        
        private bool _complete = false;

        private readonly List<StationModule> _stationModules = new List<StationModule>();

        private Hyperdrive _hyperdrive;
        private ShieldGenerator _shieldGenerator;
        private StationHull _stationHull;
        // todo add turrets

        public int resources = 0;
        
        private void Start() {
            transform.position = gridManager.GetGridCentre();
            
            _hyperdrive = new Hyperdrive(this);
            _shieldGenerator = new ShieldGenerator(this);
            _stationHull = new StationHull(this);
            
            _stationModules.Add(_hyperdrive);
            _stationModules.Add(_shieldGenerator);
            _stationModules.Add(_stationHull);
        }

        private void Update() {
            _shieldGenerator.Update();
        }

        public void GameOver(bool victory) {
            // Ensures LeaveRoom is only called once
            if (!_complete) {
                // todo play animation (station exploding/hyperdrive activating)
                string eventMessage = "Game over";
                if (victory) eventMessage = "Game completed";
                EventsManager.AddMessage(eventMessage);
                StatsManager.GameStats.victory = victory;
                StatsManager.GameStats.endTime = Time.time;
                gameManager.exitScene = Scenes.EndCutsceneScene;
                gameManager.LeaveRoom();
                _complete = true;
                Debug.Log("LEFT ROOM");
            }
        }

        public void AddResources(int resources) {
            this.resources += resources;
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
        
        public Hyperdrive GetHyperdrive() {
            return _hyperdrive;
        }

        public List<StationModule> GetModules() {
            return _stationModules;
        }

        public StationModule GetModule(RepairCommand.StationModule module) {
            switch (module) {
                case RepairCommand.StationModule.Hyperdrive:
                    return _hyperdrive;
                case RepairCommand.StationModule.Hull:
                    return _stationHull;
                case RepairCommand.StationModule.ShieldGenerator:
                    return _shieldGenerator;
                default:
                    throw new ArgumentOutOfRangeException(nameof(module), module, null);
            }
        }

    }
}
