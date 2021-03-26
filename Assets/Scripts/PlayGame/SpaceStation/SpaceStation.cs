using System;
using System.Collections.Generic;
using Photon.Pun;
using PlayGame.Speech.Commands;
using PlayGame.Stats;
using PlayGame.UI;
using Statics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace PlayGame.SpaceStation {
    public class SpaceStation : MonoBehaviourPun {

        public GridManager gridManager;
        public GameManager gameManager;
        
        private bool _complete = false;

        private readonly List<StationModule> _stationModules = new List<StationModule>();

        private Hyperdrive _hyperdrive;
        private ShieldGenerator _shieldGenerator;
        private StationHull _stationHull;
        private SolarPanels _solarPanels; // todo make solar panels do something
        private Engines _engines; // todo make engines do something
        // todo add turrets

        public int resources = 0;
        
        private void Start() {
            transform.position = gridManager.GetGridCentre();
            
            _hyperdrive = new Hyperdrive(this);
            _shieldGenerator = new ShieldGenerator(this);
            _stationHull = new StationHull(this);
            _solarPanels = new SolarPanels(this);
            _engines = new Engines(this);
            
            _stationModules.Add(_hyperdrive);
            _stationModules.Add(_shieldGenerator);
            _stationModules.Add(_stationHull);
            _stationModules.Add(_solarPanels);
            _stationModules.Add(_engines);

            if (!DebugSettings.Debug && PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < _stationModules.Count; i++)
                {
                    this.photonView.RPC("RPC_SyncStationHealth", RpcTarget.AllBuffered, _stationModules[i].moduleHealth, i);
                }

            }
        }

        [PunRPC]
        public void RPC_SyncStationHealth(int health, int index)
        {
            _stationModules[index].moduleHealth = health;
        }

        private void Update() {
            _shieldGenerator.Update();
        }

        public void GameOver(bool victory) {
            // Ensures LeaveRoom is only called once
            if (!_complete) {
                if (!DebugSettings.Debug && PhotonNetwork.IsMasterClient) photonView.RPC("RPC_GameOver", RpcTarget.AllBuffered, victory, Time.time - StatsManager.GameStats.startTime);
                else if (DebugSettings.Debug) RPC_GameOver(victory, Time.time - StatsManager.GameStats.startTime);
            }
        }
        
        [PunRPC]
        public void RPC_GameOver(bool victory, float time) {
            GameManager.gameOver = true;
            string eventMessage = "Game over";
            if (victory) eventMessage = "Game completed";
            EventsManager.AddMessage(eventMessage);
            StatsManager.GameStats.victory = victory;
            StatsManager.GameStats.gameTime = time;
            SceneManager.LoadScene(Scenes.EndCutsceneScene);
            _complete = true;
        }

        public void AddResources(int resources) {
            if (!DebugSettings.Debug) photonView.RPC("RPC_AddResources", RpcTarget.AllBuffered, resources);
            else this.resources += resources;
        }

        [PunRPC]
        public void RPC_AddResources(int resources)
        {
            this.resources += resources;
            EventsManager.AddMessage("Resources at " + this.resources.ToString());
        }

        public void TakeDamage(int damage) {
            int damageRemaining = _shieldGenerator.AbsorbDamage(damage); // Shields take as much of the damage as they can
            int moduleDamage = Random.Range(0, damageRemaining); // Random module takes a random amount of damage
            int index = Random.Range(0, _stationModules.Count);

            if (!DebugSettings.Debug) {
                photonView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, moduleDamage, index, damageRemaining);
            } else {
                _stationModules[index].TakeDamage(moduleDamage);
                damageRemaining -= moduleDamage;

                _stationHull.TakeDamage(damageRemaining); // Hull takes the rest of the damage
            }
        }

        [PunRPC]
        public void RPC_TakeDamage(int moduleDamage, int index, int damageRemaining)
        {  
            _stationModules[index].TakeDamage(moduleDamage);
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
                case RepairCommand.StationModule.Engines:
                    return _engines;
                case RepairCommand.StationModule.SolarPanels:
                    return _solarPanels;
                default:
                    throw new ArgumentOutOfRangeException(nameof(module), module, null);
            }
        }

    }
}
