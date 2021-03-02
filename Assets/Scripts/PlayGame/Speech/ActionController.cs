using System;
using System.Collections;
using System.Linq;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using PlayGame.UI;
using UnityEngine;
using Ping = PlayGame.Pings.Ping;

namespace PlayGame.Speech {
    public class ActionController {

        public SpeechRecognition speechRecognition;
        
        public GameObject player;
        public GameObject spaceStationObject;
        public Collider spaceStationCollider;
        public MoveObject moveObject;
        public MiningLaser miningLaser;
        public LaserGun laserGun;
        public PlayerData playerData;
        public SpaceStation spaceStation;
    
        public GameObject ping;
        public PingManager pingManager;

        private bool _lockedOn = false;
        private bool _shooting = false;

        public void PerformActions(Command command) {
            switch (command.GetCommandType()) {
                case Command.CommandType.Movement:
                    PerformMovementCommand((MovementCommand) command);
                    break;
                case Command.CommandType.Turn:
                    PerformTurnCommand((TurnCommand) command);
                    break;
                case Command.CommandType.Ping:
                    PerformPingCommand((PingCommand) command);
                    break;
                case Command.CommandType.Transfer:
                    PerformTransferCommand();
                    break;
                case Command.CommandType.Toggle:
                    PerformToggleCommand((ToggleCommand) command);
                    break;
                case Command.CommandType.Speed:
                    PerformSpeedCommand((SpeedCommand) command);
                    break;
                case Command.CommandType.Shoot:
                    PerformShootCommand();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PerformMovementCommand(MovementCommand command) {
            moveObject.SetSpeed(1); // Start moving
            
            switch (command.movementType) {
                case MovementCommand.MovementType.Direction:
                    moveObject.SetDirection(command.direction);
                    break;
                case MovementCommand.MovementType.Destination:
                    if (command.destinationType == MovementCommand.DestinationType.SpaceStation) {
                        moveObject.SetDestination(spaceStationObject.transform.position, spaceStationCollider);
                    } else MoveToPing();
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
                if(onlyPing.GetPingType() != PingType.None) { // Only move to ping if theres an active ping
                    moveObject.SetDestination(onlyPing.GetPositionVector());
                } 
            } else { // Dont move if there's no ping
                moveObject.SetSpeed(0);
            }
        }

        private void PerformTurnCommand(TurnCommand command) {
            switch (command.turnType) {
                case TurnCommand.TurnType.Direction:
                    moveObject.SetDirection(command.direction);
                    break;
                case TurnCommand.TurnType.Destination:
                    if (command.destinationType == MovementCommand.DestinationType.SpaceStation) {
                        moveObject.SetDestination(spaceStationObject.transform.position, spaceStationCollider);
                    } else MoveToPing();
                    break;
                case TurnCommand.TurnType.Grid:
                    moveObject.SetDestination(command.gridCoord.GetWorldVector());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PerformPingCommand(PingCommand command) {
            if (playerData.GetRole() != Role.StationCommander) return; // Only let the station commander create pings
            
            Ping newPing = new Ping(command.gridCoord, command.pingType);
            pingManager.AddPing(newPing);
        }

        private void PerformTransferCommand() {
            // Check player is in the same grid square as the station
            if (GridCoord.GetCoordFromVector(player.transform.position).Equals(GridCoord.GetCoordFromVector(spaceStationObject.transform.position))) {
                spaceStation.AddResources(playerData.GetResources()); // Add the resources into the space station
                playerData.RemoveResources(); // Remove them from the player
            } else {
                EventsManager.AddMessageToQueue("You must be next to the space station to transfer resources");
            }
        }

        private void PerformToggleCommand(ToggleCommand command) {
            switch (command.objectType) {
                case ToggleCommand.ObjectType.MiningLaser:
                    if (command.on) miningLaser.EnableMiningLaser(); 
                    else miningLaser.DisableMiningLaser();
                    break;
                case ToggleCommand.ObjectType.Lock:
                    _lockedOn = command.on;
                    if (command.on) speechRecognition.StartLockOn(GetLockTarget(command.lockTargetType));
                    else speechRecognition.StopLockOn(GetLockTarget(command.lockTargetType));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Transform GetLockTarget(ToggleCommand.LockTargetType lockTargetType) {
            if (lockTargetType == ToggleCommand.LockTargetType.Pirate) return GetNearestPirate();
            return GetNearestAsteroid();
        }

        private Transform GetNearestAsteroid() {
            throw new NotImplementedException();
        }

        private Transform GetNearestPirate() {
            return player.GetComponent<MoveObject>().GetNearestEnemyTransform();
        }

        private void PerformSpeedCommand(SpeedCommand command) {
            moveObject.SetSpeed(command.speed);
        }

        // todo shooting should be part of toggle
        private void PerformShootCommand() {
            if (_shooting) {
                laserGun.StopShooting();
                _shooting = false;
            } else {
                laserGun.StartShooting();
                _shooting = true;
            }
        }
        
        public IEnumerator LockOn(Transform lockTarget) {
            while (_lockedOn) {
                player.GetComponent<MoveObject>().FaceTarget(lockTarget);
                yield return null;
            }
        }

    }
}