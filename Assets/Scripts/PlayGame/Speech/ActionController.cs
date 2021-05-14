using System;
using System.Linq;
using Photon.Pun;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using PlayGame.UI;
using Statics;
using Tutorial;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ping = PlayGame.Pings.Ping;

namespace PlayGame.Speech {
    
    /// <summary>
    /// This class performs commands.
    /// </summary>
    public class ActionController {

        public GameObject player;
        public GameObject spaceStationObject;
        public MoveObject moveObject;
        public MiningLaser miningLaser;
        public LaserGun laserGun;
        public PlayerData playerData;
        public SpaceStation.SpaceStation spaceStation;

        public PingManager pingManager;

        /// <summary>
        /// Checks that the players role is allowed to performs the command.
        /// <para>If it is allowed, perform the command.</para>
        /// </summary>
        /// <param name="command"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the command has an invalid command type.</exception>
        public void PerformActions(Command command) {
            if (SceneManager.GetActiveScene().name == Scenes.TutorialScene) TutorialManager.LatestCommand = command;

            if (!DebugSettings.SinglePlayer) {
                if (playerData.GetRole() != Role.StationCommander && command.IsCommanderOnly()) {
                    // Prevent players from performing station commander commands
                    if (playerData.photonView.IsMine)
                        EventsManager.AddMessage("Only the station commander can perform that command!");
                    return;
                }

                if (playerData.GetRole() != Role.Miner && command.IsMinerOnly()) {
                    // Prevent commander from performing miner commands
                    if (playerData.photonView.IsMine) EventsManager.AddMessage("Only miners can perform that command!");
                    return;
                }
            }

            if (playerData.dead) return; // Dead players can't perform actions
            
            switch (command.GetCommandType()) {
                case Command.CommandType.Movement:
                    PerformMovementCommand((MovementCommand) command);
                    break;
                case Command.CommandType.Ping:
                    PerformPingCommand((PingCommand) command);
                    break;
                case Command.CommandType.Transfer:
                    PerformTransferCommand((TransferCommand) command);
                    break;
                case Command.CommandType.Toggle:
                    PerformToggleCommand((ToggleCommand) command);
                    break;
                case Command.CommandType.Speed:
                    PerformSpeedCommand((SpeedCommand) command);
                    break;
                case Command.CommandType.Repair:
                    PerformRepairCommand((RepairCommand) command);
                    break;
                case Command.CommandType.Respawn:
                    PerformRespawnCommand();
                    break;
                case Command.CommandType.Yes:
                    break;
                case Command.CommandType.No:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Respawns a random dead player and increases the respawn cost if the station has enough resources.
        /// <remarks>This method can only be called by the host.</remarks>
        /// </summary>
        private void PerformRespawnCommand() {
            if (!PhotonNetwork.IsMasterClient) return;
            if (spaceStation.resources < spaceStation.GetRespawnCost()) { // If the station doesn't have enough resources dont respawn
                EventsManager.AddMessage(spaceStation.GetRespawnCost() + " resources required.");
                return;
            }
            
            spaceStation.AddResources(-spaceStation.GetRespawnCost()); // Remove resources
            spaceStation.IncreaseRespawnCost();
            
            PlayerData p = PlayerData.GetRandomDeadPlayer();
            if (p != null) playerData.RespawnPlayer(p.GetPlayerID()); // Respawn a random dead player
        }

        /// <summary>
        /// Repairs the specified station module.
        /// </summary>
        /// <param name="command"></param>
        private void PerformRepairCommand(RepairCommand command) {
            int repairAmount;
            if (command.repairAmount != null) repairAmount = (int) command.repairAmount;
            else repairAmount = spaceStation.resources; // If no repair amount is given repair the module as much as possible
            
            spaceStation.GetModule(command.stationModule).Repair(repairAmount);
        }

        /// <summary>
        /// Returns the destinations transform based on the commands destination type.
        /// </summary>
        /// <param name="command"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the command has an invalid destination type.</exception>
        private Transform GetDestination(MovementCommand command) {
            switch (command.destinationType) {
                case MovementCommand.DestinationType.SpaceStation:
                    return spaceStationObject.transform;
                case MovementCommand.DestinationType.Pirate:
                    return GetNearestPirate();
                case MovementCommand.DestinationType.Asteroid:
                    return GetNearestAsteroid();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Performs the given movement command.
        /// </summary>
        /// <param name="command"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the command has an invalid movement type.</exception>
        private void PerformMovementCommand(MovementCommand command) {
            if (!command.turnOnly) moveObject.SetSpeed(1); // Start moving if not a turn command

            switch (command.movementType) {
                case MovementCommand.MovementType.Direction:
                    if (command.turn == MovementCommand.TurnType.None) moveObject.SetDirection(command.direction, false); // Move without turning the player
                    if (command.turn == MovementCommand.TurnType.Instant) moveObject.SetDirection(command.direction, true); // Move and player faces direction of movement
                    if (command.turn == MovementCommand.TurnType.Smooth) moveObject.StartRotating(command.direction); // Rotate the player
                    break;
                case MovementCommand.MovementType.Destination:
                    if (command.destinationType == MovementCommand.DestinationType.Ping) {
                        MoveToPing();
                    } else {
                        Transform destination = GetDestination(command);
                        moveObject.SetDestination(destination.position, destination.GetComponent<Collider>());
                    }

                    break;
                case MovementCommand.MovementType.Grid:
                    moveObject.SetDestination(command.gridCoord.GetWorldVector());
                    break;
                default:
                    moveObject.SetSpeed(0); // Dont move if theres an error
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Check that there is only one ping and if so move to the ping
        /// </summary>
        /// TODO: somehow number pings so that the player can go to a specific one
        private void MoveToPing() {
            var pings = pingManager.GetPings();
            if (pings.Count == 1) {
                Ping onlyPing = pings.Keys.ToList()[0];
                if (onlyPing.GetPingType() != PingType.None) { // Only move to ping if theres an active ping
                    moveObject.SetDestination(onlyPing.GetPositionVector());
                }
            } else { // Dont move if there's no ping
                moveObject.SetSpeed(0);
            }
        }

        /// <summary>
        /// Creates a new ping with the given location and type.
        /// </summary>
        /// <param name="command"></param>
        private void PerformPingCommand(PingCommand command) {
            Ping newPing = new Ping(command.gridCoord, command.pingType);
            pingManager.AddPing(newPing);
        }

        /// <summary>
        /// Transfers the players resources to the station if the player is close enough to the station.
        /// <remarks>This method can only be called by the local player.</remarks>
        /// </summary>
        /// <param name="command"></param>
        private void PerformTransferCommand(TransferCommand command) {
            if ((!DebugSettings.Debug && player.GetPhotonView().IsMine) || DebugSettings.Debug) {
                // Check player is in the same grid square as the station
                if (moveObject.NearStation()) {
                    spaceStation.AddResources(command.TransferAmount); // Add the resources into the space station
                    playerData.RemoveResources(command.TransferAmount); // Remove them from the player
                } else EventsManager.AddMessage("You must be next to the space station to transfer resources");
            }
        }

        /// <summary>
        /// Performs the given toggle command.
        /// </summary>
        /// <param name="command"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the command has an invalid object type.</exception>
        private void PerformToggleCommand(ToggleCommand command) {
            switch (command.objectType) {
                case ToggleCommand.ObjectType.MiningLaser:
                    if (command.on) moveObject.SetLockTargetType(ToggleCommand.LockTargetType.Asteroid); // If mining lock to asteroid
                    else moveObject.SetLockTargetType(ToggleCommand.LockTargetType.None); // If stopping mining disable lock
                    break;
                case ToggleCommand.ObjectType.Lock:
                    moveObject.SetLockTargetType(command.lockTargetType);
                    break;
                case ToggleCommand.ObjectType.LaserGun:
                    if (command.on) moveObject.SetLockTargetType(ToggleCommand.LockTargetType.Pirate); // If shooting lock to pirate
                    else moveObject.SetLockTargetType(ToggleCommand.LockTargetType.None); // If stopping shooting disable lock
                    break;
                case ToggleCommand.ObjectType.Hyperdrive:
                    if (command.on) spaceStation.GetHyperdrive().Activate(); // Once activated the game will finish so it cannot be deactivated
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns the transform of the nearest asteroid to the player.
        /// </summary>
        /// <returns></returns>
        private Transform GetNearestAsteroid() {
            return player.GetComponent<MoveObject>().GetNearestAsteroidTransform();
        }

        /// <summary>
        /// Returns the transform of the nearest pirate to the player.
        /// </summary>
        /// <returns></returns>
        private Transform GetNearestPirate() {
            return player.GetComponent<MoveObject>().GetNearestEnemyTransform();
        }

        /// <summary>
        /// Sets the players speed to the fraction specified in the command.
        /// <para>If the speed is 0, stop the player from rotating and disable the lock.</para>
        /// </summary>
        /// <param name="command"></param>
        private void PerformSpeedCommand(SpeedCommand command) {
            moveObject.SetSpeed(command.Speed);
            if (command.Speed == 0) {
                moveObject.SetLockTargetType(ToggleCommand.LockTargetType.None);
                moveObject.StopRotating();
            }
        }
    }
}