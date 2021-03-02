using System;
using System.Collections;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
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
                    moveObject.SetDestination(command.destination); // todo destination collider
                    break;
                case MovementCommand.MovementType.Grid:
                    moveObject.SetDestination(command.gridCoord.GetWorldVector());
                    break;
                default:
                    moveObject.SetSpeed(0); // Dont move if theres an error
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PerformTurnCommand(TurnCommand command) {
            switch (command.turnType) {
                case TurnCommand.TurnType.Direction:
                    moveObject.SetDirection(command.direction);
                    break;
                case TurnCommand.TurnType.Destination:
                    moveObject.SetDestination(command.destination);
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
                    if (command.on) speechRecognition.StartLockOn(command.lockTarget);
                    else speechRecognition.StopLockOn(command.lockTarget);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        
        public IEnumerator LockOn(GameObject lockTarget) {
            while (_lockedOn) {
                // Transform transform = player.GetComponent<MoveObject>().GetNearestEnemyTransform();
                player.GetComponent<MoveObject>().FaceTarget(lockTarget.transform);
                yield return null;
            }
        }

    }
}