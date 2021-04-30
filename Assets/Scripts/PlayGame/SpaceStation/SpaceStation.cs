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
    
    /// <summary>
    /// This class controls the space station.
    /// </summary>
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

        /// <summary>
        /// Initialises the station modules and performs RPC calls to sync the station modules starting health if the player is the host.
        /// </summary>
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

            if (!DebugSettings.Debug && PhotonNetwork.IsMasterClient) {
                for (int i = 0; i < _stationModules.Count; i++) {
                    photonView.RPC(nameof(RPC_SyncStationHealth), RpcTarget.AllBuffered, _stationModules[i].ModuleHealth, i);
                }
            }
        }

        /// <summary>
        /// Sets the modules health to the given value.
        /// <remarks>Method is called via RPC.</remarks>
        /// </summary>
        /// <param name="health"></param>
        /// <param name="index">The module index.</param>
        [PunRPC]
        public void RPC_SyncStationHealth(int health, int index) {
            _stationModules[index].ModuleHealth = health;
        }
        
        private void Update() {
            _shieldGenerator.Update();
        }

        /// <summary>
        /// Performs an RPC call to increase the stations resources.
        /// </summary>
        /// <param name="amount"></param>
        public void AddResources(int amount) {
            if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_AddResources), RpcTarget.AllBuffered, amount);
            else resources += amount;
        }

        /// <summary>
        /// Increases the stations resources.
        /// <remarks>Method is called via RPC.</remarks>
        /// </summary>
        /// <param name="amount"></param>
        [PunRPC]
        public void RPC_AddResources(int amount) {
            resources += amount;
            EventsManager.AddMessage("Resources at " + resources);
        }

        /// <summary>
        /// Performs RPC calls to damage the space station.
        /// <para>The shields absorb as much of the damage as they can.</para>
        /// A random amount of the remaining damage is assigned to a random station module.
        /// <para>The rest of the damage is dealt to the hull.</para>
        /// </summary>
        /// <param name="damage"></param>
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

        /// <summary>
        /// Damages the space station.
        /// <remarks>Method is called via RPC.</remarks>
        /// </summary>
        /// <param name="moduleDamage">The amount of damage to deal to the random module.</param>
        /// <param name="index">The random modules index.</param>
        /// <param name="damageRemaining">The amount of damage to deal to the hull.</param>
        [PunRPC]
        public void RPC_TakeDamage(int moduleDamage, int index, int damageRemaining) {  
            _stationModules[index].TakeDamage(moduleDamage);
            damageRemaining -= moduleDamage;

            _stationHull.TakeDamage(damageRemaining); // Hull takes the rest of the damage
        }

        /// <summary>
        /// Returns the hull StationModule.
        /// </summary>
        public StationModule GetStationHull() {
            return _stationHull;
        }
        
        /// <summary>
        /// Returns the hyperdrive StationModule.
        /// </summary>
        public Hyperdrive GetHyperdrive() {
            return _hyperdrive;
        }

        /// <summary>
        /// Returns the list of all StationModules.
        /// </summary>
        public List<StationModule> GetModules() {
            return _stationModules;
        }

        /// <summary>
        /// Returns a StationModule based on a given <c>RepairCommand.StationModule</c>.
        /// </summary>
        /// <param name="module">The type of module to return.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if module is not a valid <c>RepairCommand.StationModule</c>.</exception>
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

        /// <summary>
        /// Performs an RPC call to increase the resource cost of respawning a player.
        /// </summary>
        public void IncreaseRespawnCost() {
            if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_IncreaseRespawnCost), RpcTarget.AllBuffered);
            else _respawnCost += GameConstants.PlayerRespawnCostIncrease;
        }
        
        /// <summary>
        /// Increases the resource cost of respawning a player.
        /// <remarks>Method is called via RPC.</remarks>
        /// </summary>
        [PunRPC]
        public void RPC_IncreaseRespawnCost() {
            _respawnCost += GameConstants.PlayerRespawnCostIncrease;
        }
        
        /// <summary>
        /// Returns the resource cost of respawning a player.
        /// </summary>
        public int GetRespawnCost() {
            return _respawnCost;
        }

    }
}
