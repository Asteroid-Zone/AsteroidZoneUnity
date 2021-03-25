using System;
using System.Linq;
using Photon.Pun;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using PlayGame.UI;
using Statics;
using UnityEngine;
using Ping = PlayGame.Pings.Ping;

namespace PlayGame.Speech {
    public class ActionController {

        public GameObject player;
        public GameObject spaceStationObject;
        public MoveObject moveObject;
        public MiningLaser miningLaser;
        public LaserGun laserGun;
        public PlayerData playerData;
        public SpaceStation.SpaceStation spaceStation;

        public PingManager pingManager;

        public void PerformActions(Command command) {
            if (playerData.GetRole() != Role.StationCommander && command.IsCommanderOnly()) return; // Prevent players from performing station commander commands
            
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PerformRepairCommand(RepairCommand command) {
            int repairAmount;
            if (command.repairAmount != null) repairAmount = (int) command.repairAmount;
            else repairAmount = spaceStation.resources; // If no repair amount is given repair the module as much as possible
            
            spaceStation.GetModule(command.stationModule).Repair(repairAmount);
        }

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

        // Check whether there is only one ping and if so go to the ping
        // TODO: somehow number pings so that the player can go to a specific one
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

        private void PerformPingCommand(PingCommand command) {
            Ping newPing = new Ping(command.gridCoord, command.pingType);
            pingManager.AddPing(newPing);
        }

        private void PerformTransferCommand(TransferCommand command) {
            // Check player is in the same grid square as the station
            if (GridCoord.GetCoordFromVector(player.transform.position).Equals(GridCoord.GetCoordFromVector(spaceStationObject.transform.position))) {
                if ((!DebugSettings.Debug && player.GetPhotonView().IsMine) || DebugSettings.Debug) {
                    spaceStation.AddResources(playerData.GetResources()); // Add the resources into the space station
                    playerData.RemoveResources(playerData.GetResources()); // Remove them from the player
                }
            } else {
                EventsManager.AddMessage("You must be next to the space station to transfer resources");
            }
        }

        private void PerformToggleCommand(ToggleCommand command) {
            switch (command.objectType) {
                case ToggleCommand.ObjectType.MiningLaser:
                    if (command.on) miningLaser.EnableMiningLaser();
                    else miningLaser.DisableMiningLaser();
                    break;
                case ToggleCommand.ObjectType.Lock:
                    moveObject.SetLockTargetType(command.lockTargetType);
                    break;
                case ToggleCommand.ObjectType.LaserGun:
                    if (command.on) laserGun.StartShooting();
                    else laserGun.StopShooting();
                    break;
                case ToggleCommand.ObjectType.Hyperdrive:
                    if (command.on) spaceStation.GetHyperdrive().Activate(); // Once activated the game will finish so it cannot be deactivated
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Transform GetNearestAsteroid() {
            return player.GetComponent<MoveObject>().GetNearestAsteroidTransform();
        }

        private Transform GetNearestPirate() {
            return player.GetComponent<MoveObject>().GetNearestEnemyTransform();
        }

        private void PerformSpeedCommand(SpeedCommand command) {
            moveObject.SetSpeed(command.speed);
            if (command.speed == 0) moveObject.StopRotating();
        }
    }
}