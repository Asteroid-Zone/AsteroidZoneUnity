using System;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using UnityEngine;
using Ping = PlayGame.Pings.Ping;

namespace PlayGame.Speech {
    public class ActionController
    {

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
                    break;
                case Command.CommandType.Speed:
                    break;
                case Command.CommandType.Shoot:
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
        
    }
}
























