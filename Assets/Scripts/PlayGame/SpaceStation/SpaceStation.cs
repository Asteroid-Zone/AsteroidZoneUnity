using System;
using System.Collections.Generic;
using Photon.Pun;
using PlayGame.Speech.Commands;
using PlayGame.UI;
using Statics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace PlayGame.SpaceStation {
    public class SpaceStation : MonoBehaviourPun {

        public UnityEvent stationAttacked;
        
        private bool _complete = false;
        
        private readonly List<StationModule> _stationModules = new List<StationModule>();

        private Hyperdrive _hyperdrive;
        private ShieldGenerator _shieldGenerator;
        private StationHull _stationHull;
        private SolarPanels _solarPanels; // todo make solar panels do something
        private Engines _engines; // todo make engines do something
        // todo add turrets

        public int resources = 0;

        public GameManager gameManager;

        private int _respawnCost;

        private void Start() {
            gameManager = GameObject.FindGameObjectWithTag(Tags.GameManager).GetComponent<GameManager>();
            transform.position = GridManager.GetGridCentre();

            _respawnCost = GameConstants.PlayerRespawnCost;
            
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
                    photonView.RPC(nameof(RPC_SyncStationHealth), RpcTarget.AllBuffered, _stationModules[i].ModuleHealth, i);
                }

            }
        }

        [PunRPC]
        public void RPC_SyncStationHealth(int health, int index)
        {
            _stationModules[index].ModuleHealth = health;
        }

        private void Update() {
            _shieldGenerator.Update();
        }

        public void AddResources(int amount) {
            if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_AddResources), RpcTarget.AllBuffered, amount);
            else resources += amount;
        }

        [PunRPC]
        public void RPC_AddResources(int amount) {
            resources += amount;
            EventsManager.AddMessage("Resources at " + resources);
        }

        public void TakeDamage(int damage) {
            int damageRemaining = _shieldGenerator.AbsorbDamage(damage); // Shields take as much of the damage as they can
            int moduleDamage = Random.Range(0, damageRemaining); // Random module takes a random amount of damage
            int index = Random.Range(0, _stationModules.Count);
            stationAttacked.Invoke();
            

            if (!DebugSettings.Debug) {
                photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.AllBuffered, moduleDamage, index, damageRemaining);
            } else {
                _stationModules[index].TakeDamage(moduleDamage);
                damageRemaining -= moduleDamage;

                _stationHull.TakeDamage(damageRemaining); // Hull takes the rest of the damage
            }
        }

        [PunRPC]
        public void RPC_TakeDamage(int moduleDamage, int index, int damageRemaining) {  
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

        public void IncreaseRespawnCost() {
            if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_IncreaseRespawnCost), RpcTarget.AllBuffered);
            else _respawnCost += GameConstants.PlayerRespawnCostIncrease;
        }
        
        [PunRPC]
        public void RPC_IncreaseRespawnCost() {
            _respawnCost += GameConstants.PlayerRespawnCostIncrease;
        }
        
        public int GetRespawnCost() {
            return _respawnCost;
        }

    }
}
