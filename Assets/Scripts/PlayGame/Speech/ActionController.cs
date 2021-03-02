using System;
using PlayGame.Pings;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using UnityEngine;

namespace PlayGame.Speech {
    public class ActionController {
        
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
                    PerformMovement((MovementCommand) command);
                    break;
                case Command.CommandType.Turn:
                    PerformTurn((TurnCommand) command);
                    break;
                case Command.CommandType.Ping:
                    break;
                case Command.CommandType.Transfer:
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

        private void PerformMovement(MovementCommand command) {
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

        private void PerformTurn(TurnCommand command) {
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
        
    }
}