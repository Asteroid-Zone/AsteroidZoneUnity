using System;
using System.Collections.Generic;
using Photon.GameControllers;
using Photon.Pun;
using PlayGame.Camera;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using PlayGame.Stats;
using PlayGame.UI;
using Statics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace PlayGame.Player {
    
    /// <summary>
    /// Player roles
    /// </summary>
    public enum Role {
        StationCommander,
        Miner
    }

    /// <summary>
    /// Player quest types
    /// </summary>
    public enum QuestType {
        MineAsteroids,
        ReturnToStationResources,
        TransferResources,
        PirateWarning,
        ReturnToStationDefend,
        DefendStation,
        HelpPlayers,
        RepairStation,
        ActivateHyperdrive,
        FindAsteroids,
        Respawn
    }

    /// <summary>
    /// This class controls the players data.
    /// </summary>
    public class PlayerData : MonoBehaviourPun {
        
        public static List<GameObject> Players;
        private int _playerID;

        public bool dead;

        public Role role;

        public GameObject viewableArea;
        public GameObject viewableAreaMinimap;

        private int _miningLevel;
        private int _miningXP;
        private int _combatLevel;
        private int _combatXP;
        
        private int _maxHealth;
        private int _health;
    
        private float _maxSpeed;
        private float _rotateSpeed;

        private int _laserSpeed;
        private int _laserDamage;
        private int _laserRange;
        private int _laserDamageRange;

        private float _lookRadius;

        private int _resources;

        private NavMeshAgent _playerAgent;

        private PlayerStats _playerStats;

        public QuestType currentQuest;

        public MeshRenderer shipModel;
        public MeshRenderer gunLeft;
        public MeshRenderer gunRight;

        private Transform _spaceStation;

        private LaserGun _laserGun;
        private MiningLaser _miningLaser;
        private MoveObject _moveObject;

        private GameObject _deadPanel;
        
        private void Start() {
            _spaceStation = GameObject.FindGameObjectWithTag(Tags.StationTag).transform;
            _playerAgent = GetComponent<NavMeshAgent>();
            DontDestroyOnLoad(gameObject);

            // Initialise the players stats
            _playerStats = new PlayerStats();
            if (!DebugSettings.Debug) _playerStats.photonID = photonView.ViewID;
            _playerStats.playerName = PhotonNetwork.NickName;
            _playerStats.role = role;
            StatsManager.PlayerStatsList.Add(_playerStats);

            // Initialise the players list
            UpdatePlayerLists();
            if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_UpdatePlayerLists), RpcTarget.Others);

            // Set camera to cockpit for miners and tactical for station commander, if single player play in cockpit mode
            if (photonView.IsMine) {
                bool cockpitMode = role == Role.Miner || DebugSettings.SinglePlayer;
                GameObject.FindGameObjectWithTag(Tags.CameraManagerTag).GetComponent<CameraManager>().SetMode(cockpitMode);
            }

            // Set up the player
            if (role == Role.StationCommander && !DebugSettings.SinglePlayer) SetUpStationCommander();
            else SetUpMiner(true);
        }

        /// <summary>
        /// Gets the miners components and initialises values.
        /// </summary>
        /// <param name="initial">True if this is the initial game set up. False is used for resetting after respawning.</param>
        private void SetUpMiner(bool initial) {
            if (initial) {
                _laserGun = gameObject.GetComponent<LaserGun>();
                _miningLaser = gameObject.GetComponent<MiningLaser>();
                _moveObject = gameObject.GetComponent<MoveObject>();
                
                if (photonView.IsMine) _deadPanel = GameObject.FindGameObjectWithTag(Tags.DeadPanel);

                // Levels do not reset when player dies, xp towards leveling up does get reset
                _miningLevel = 0;
                _combatLevel = 0;

                _laserDamageRange = GameConstants.PlayerLaserDamageRange;
                
                _maxHealth = GameConstants.PlayerMaxHealth;
                _maxSpeed = GameConstants.PlayerMaxSpeed;
                _rotateSpeed = GameConstants.PlayerRotateSpeed;

                _laserSpeed = GameConstants.PlayerLaserSpeed;
                _laserDamage = GameConstants.PlayerLaserDamage;
                _laserRange = GameConstants.PlayerLaserRange;

                _lookRadius = GameConstants.PlayerLookRadius;
                
                _playerID = (_playerStats.photonID / 1000) - 1;
                //   _playerID = _playerID > 3 ? 3 : _playerID;
                //    _playerID = _playerID < 0 ? 0 : _playerID;
                SetPlayerColour();
            }
            
            if (photonView.IsMine) _deadPanel.SetActive(false);

            _miningXP = 0;
            _combatXP = 0;
            
            // The current speed of the player is will be stored in the speed of its NavMeshAgent
            _playerAgent.speed = 0;
            _resources = 0;
            _health = _maxHealth;

            ResizeViewableArea();
            
            currentQuest = QuestType.MineAsteroids;

            if (photonView.IsMine) transform.position = GameSetup.Instance.spawnPoints[_playerID].position; // Set spawn point
        }

        /// <summary>
        /// Sets the station commanders initial position and starting quest.
        /// </summary>
        private void SetUpStationCommander() {
            transform.position = GridManager.GetGridCentre();
            gameObject.transform.position = _spaceStation.position;
            currentQuest = QuestType.HelpPlayers;
        }

        /// <summary>
        /// Sets each players ship to be a different colour.
        /// </summary>
        private void SetPlayerColour() {
            Color colour = Color.cyan;
            switch (_playerID) {
                case 1:
                    colour = Color.red;
                    break;
                case 2:
                    colour = Color.green;
                    break;
                case 3:
                    colour = Color.yellow;
                    break;
            }

            shipModel.material.color = colour;
            gunLeft.material.color = colour;
            gunRight.material.color = colour;
        }

        /// <summary>
        /// Returns the players progress towards the next combat level.
        /// <remarks>Between 0 and 1.</remarks>
        /// </summary>
        public float GetCombatLevelProgress() {
            float levelUpThreshold = GameConstants.InitialLevelUpThreshold + (_combatLevel * GameConstants.LevelUpScaleAmount);
            return _combatXP / levelUpThreshold;
        }

        /// <summary>
        /// Returns the players current combat level.
        /// </summary>
        public int GetCombatLevel() {
            return _combatLevel;
        }

        /// <summary>
        /// Increases the players combat xp.
        /// <para>Performs an RPC call to level up the player if they have enough xp.</para>
        /// <remarks>This method can only be called if this instance belongs to the local player.</remarks>
        /// </summary>
        /// <param name="amount">Amount to increase the xp by.</param>
        public void IncreaseCombatXP(int amount) {
            if (!photonView.IsMine) return; // Each player keeps track of their own xp, levels are rpc called
            
            _combatXP += amount;

            int levelUpThreshold = GameConstants.InitialLevelUpThreshold + (_combatLevel * GameConstants.LevelUpScaleAmount);
            if (_combatXP > levelUpThreshold) {
                _combatXP = 0;
                
                if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_LevelUpCombat), RpcTarget.AllBuffered);
                else RPC_LevelUpCombat();
            }
        }

        /// <summary>
        /// Increases the players combat level and improves their laser gun.
        /// </summary>
        [PunRPC]
        public void RPC_LevelUpCombat() {
            _combatLevel += 1;
            
            _laserGun.ReduceShotDelay(20); // Reduce shot delay by 20ms
            _laserDamage += 1;
            _laserRange += 1;

            // Only change every other level
            if (_combatLevel % 2 == 0) {
                _laserDamageRange -= 1;
            }
        }
        
        /// <summary>
        /// Returns the players progress towards the next mining level.
        /// <remarks>Between 0 and 1.</remarks>
        /// </summary>
        public float GetMiningLevelProgress() {
            float levelUpThreshold = GameConstants.InitialLevelUpThreshold + (_miningLevel * GameConstants.LevelUpScaleAmount);
            return _miningXP / levelUpThreshold;
        }

        /// <summary>
        /// Returns the players current mining level.
        /// </summary>
        public int GetMiningLevel() {
            return _miningLevel;
        }
        
        /// <summary>
        /// Increases the players mining xp.
        /// <para>Performs an RPC call to level up the player if they have enough xp.</para>
        /// <remarks>This method can only be called if this instance belongs to the local player.</remarks>
        /// </summary>
        /// <param name="amount">Amount to increase the xp by.</param>
        public void IncreaseMiningXP(int amount) {
            if (!photonView.IsMine) return; // Each player keeps track of their own xp, levels are rpc called
            
            _miningXP += amount;

            int levelUpThreshold = GameConstants.InitialLevelUpThreshold + (_miningLevel * GameConstants.LevelUpScaleAmount);
            if (_miningXP > levelUpThreshold) {
                _miningXP = 0;
                
                if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_LevelUpMining), RpcTarget.AllBuffered);
                else RPC_LevelUpMining();
            }
        }

        /// <summary>
        /// Increases the players mining level and improves their mining laser.
        /// </summary>
        [PunRPC]
        public void RPC_LevelUpMining() {
            _miningLevel += 1;
            
            _miningLaser.IncreaseMiningRange(1);

            // Only change every other level
            if (_miningLevel % 2 == 0) {
                _miningLaser.IncreaseMiningRate(1);
                _miningLaser.ReduceMiningDelay(1); // todo convert to ms from frames
            }
        }

        /// <summary>
        /// Performs an RPC call to respawn a player with a given photonID.
        /// <remarks>This method can only be called by the host.</remarks>
        /// </summary>
        /// <param name="playerID">The photonID of the player to respawn.</param>
        public void RespawnPlayer(int playerID) {
            if (!PhotonNetwork.IsMasterClient) return;
            photonView.RPC(nameof(RPC_RespawnPlayer), RpcTarget.AllBuffered, playerID);
        }
        
        /// <summary>
        /// Respawns the player with the given photonID.
        /// </summary>
        /// <param name="playerID">The photonID of the player to respawn.</param>
        [PunRPC]
        public void RPC_RespawnPlayer(int playerID) {
            foreach (GameObject o in Players) {
                PlayerData p = o.GetComponent<PlayerData>();
                if (p.GetPlayerID() == playerID) p.Respawn();
            }
        }

        /// <summary>
        /// Respawns this player.
        /// </summary>
        private void Respawn() {
            dead = false;
            SetUpMiner(false);
            SetActiveRecursively(shipModel.gameObject, true);

            _playerStats.numberOfTimesRespawned++;
        }

        /// <summary>
        /// Sets the size of the viewable area ring and minimap ring.
        /// <para>Disables both if fog of war is disabled or this is not the local player.</para>
        /// </summary>
        private void ResizeViewableArea() {
            if (!DebugSettings.FogOfWar || !photonView.IsMine) {
                viewableArea.SetActive(false);
                viewableAreaMinimap.SetActive(false);
            } else {
                int size = ((int) GetLookRadius() * 2) / 10;
                viewableArea.transform.localScale = new Vector3(size, size, size);
                size *= 8;
                viewableAreaMinimap.transform.localScale = new Vector3(size, size, size);
            }
        }

        /// <summary>
        /// Updates the list of players.
        /// <remarks>Method is called via RPC.</remarks>
        /// </summary>
        [PunRPC]
        public void RPC_UpdatePlayerLists() {
            UpdatePlayerLists();
        }

        /// <summary>
        /// Updates the list of players.
        /// </summary>
        public static void UpdatePlayerLists() {
            Players = new List<GameObject>();
            Players.AddRange(GameObject.FindGameObjectsWithTag(Tags.PlayerTag));
            
            // Add the station commander only if there is one
            GameObject stationCommander = GameObject.FindGameObjectWithTag(Tags.StationCommanderTag);
            if (stationCommander != null) {
                Players.Add(stationCommander);
            }
        }

        /// <summary>
        /// Checks if the player is dead if they are a miner.
        /// <para>Updates the players position to the stations position if they are the commander.</para>
        /// </summary>
        private void Update() {
            if (role != Role.StationCommander) {
                if (!dead && _health <= 0) Die();
            } else {
                if (_spaceStation != null) gameObject.transform.position = _spaceStation.position;
            }
        }

        
        private void Die() {
            dead = true;
            
            if (photonView.IsMine) {
                _deadPanel.SetActive(true);
                EventsManager.AddMessage("YOU DIED");
            } else EventsManager.AddMessage(_playerStats.playerName + " DIED");
            
            SetActiveRecursively(shipModel.gameObject, false); // Hide the ship model
            
            _laserGun.StopShooting();
            _miningLaser.DisableMiningLaser();
            _moveObject.SetSpeed(0);
            _moveObject.SetLockTargetType(ToggleCommand.LockTargetType.None);
        }
        
        private static void SetActiveRecursively(GameObject o, bool active) {
            o.SetActive(active);

            foreach (Transform child in o.transform){
                SetActiveRecursively(child.gameObject, active);
            }
        }

        public static List<GameObject> GetList() {
            return Players;
        }
        
        public int GetResources() {
            return _resources;
        }

        public void AddResources(int resources) {
            StatsManager.GameStats.resourcesHarvested += resources;
            _resources += resources;
            _playerStats.resourcesHarvested += resources;
        }
        
        public void RemoveResources(int amount) {
            if (!DebugSettings.Debug) photonView.RPC(nameof(RPC_RemoveResources), RpcTarget.AllBuffered, amount);
            else _resources -= amount;
        }

        [PunRPC]
        public void RPC_RemoveResources(int amount) {
            _resources -= amount;
        }

        public Role GetRole() {
            return role;
        }
    
        public float GetMaxSpeed() {
            return _maxSpeed;
        }

        public float GetSpeed() {
            return _playerAgent.speed;
        }

        public float GetRotateSpeed() {
            return _rotateSpeed;
        }

        public void SetSpeed(float fraction) {
            if (fraction < 0) fraction = 0;
            if (fraction > 1) fraction = 1;
            _playerAgent.speed = fraction * _maxSpeed;
            
            if (fraction == 0) {
                _playerAgent.enabled = false;
            }
        }

        public int GetLaserSpeed() {
            return _laserSpeed;
        }

        public int GetMaxHealth() {
            return _maxHealth;
        }

        public int GetHealth() {
            return _health;
        }

        public int GetLaserRange() {
            return _laserRange;
        }

        // todo add modifier to this if adding powerups
        public float GetLookRadius() {
            return _lookRadius;
        }

        public QuestType GetQuest()
        {
            return currentQuest;
        }

        public void SetQuest(QuestType quest)
        {
            currentQuest = quest;
        }

        public int GetLaserDamage() {
            // Make the amount of damage vary a bit
            return _laserDamage + Random.Range(-_laserDamageRange, _laserDamageRange + 1);
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health < 0)
            {
                _health = 0;
            }
        }
        
        public void SetFinalLevels() {
            _playerStats.finalCombatLevel = _combatLevel;
            _playerStats.finalMiningLevel = _miningLevel;
        }

        public int GetPlayerID() {
            return _playerID;
        }

        public static PlayerData GetPlayerWithID(int photonID) {
            foreach (GameObject player in Players) {
                if (player.GetPhotonView().ViewID == photonID) return player.GetComponent<PlayerData>();
            }

            throw new ArgumentException("Invalid photon id - No player was found with photon id (" + photonID + ")");
        }

        public static PlayerData GetRandomDeadPlayer() {
            List<PlayerData> deadPlayers = new List<PlayerData>();
            
            foreach (GameObject player in Players) {
                PlayerData playerData = player.GetComponent<PlayerData>();
                if (playerData.dead) deadPlayers.Add(playerData);
            }

            if (deadPlayers.Count == 0) return null;

            int randomInt = Random.Range(0, deadPlayers.Count);
            return deadPlayers[randomInt];
        }

        public static PlayerData GetMyPlayerData()
        {
            // Gets the PlayerData object of the current (mine) player
            return DebugSettings.Debug
                ? Players[0].GetComponent<PlayerData>()
                : PhotonPlayer.Instance.myAvatar.GetComponent<PlayerData>();
        }

    }
}