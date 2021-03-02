using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayGame.Speech.Commands;
using UnityEngine;

namespace PlayGame.Speech {
    public static class Grammar {
        
        private const string GridCoordRegex = @"[a-z]( )?(\d+)";

        private static readonly List<string> MovementCommands = new List<string>{"move", "go"};
        private static readonly List<string> TurnCommands = new List<string>{"face", "turn"};
        
        // Todo add left/right commands
        private static readonly List<string> CompassDirections = new List<string>{"north", "east", "south", "west"};
        private static readonly List<List<string>> Directions = new List<List<string>>{CompassDirections};
        
        private static readonly List<string> Destinations = new List<string>{"station", "ping"};
        
        private static readonly List<string> PingTypes = new List<string>{"none", "asteroid", "pirate"};
        private static readonly List<string> PingCommands = new List<string>{"ping", "pin", "mark"};

        private static readonly List<string> SpeedCommands = new List<string>{"stop", "go"};
        
        private static readonly List<string> TransferCommands = new List<string>{"transfer", "deposit"};
        private static readonly List<string> Transferable = new List<string>{"resources", "materials"};

        private static readonly List<string> MiningLaser = new List<string>{"mining laser", "laser", "mining beam"};
        
        private static readonly List<string> LockCommands = new List<string>{"lock", "aim"};
        // private static readonly List<string> Pirate; todo create lists for synonyms
        private static readonly List<string> LockTargets = new List<string>{"pirate", "enemy", "asteroid"};

        private static readonly List<string> OnCommands = new List<string>{"activate", "engage", "turn on", "lock"};
        private static readonly List<string> OffCommands = new List<string>{"deactivate", "disengage", "turn off"};
        private static readonly List<List<string>> Activatable = new List<List<string>>{MiningLaser, LockCommands};
        
        private static readonly List<string> ShootCommands = new List<string>{"shoot", "fire"};

        private static readonly List<List<string>> SingleCommands = new List<List<string>>{SpeedCommands, ShootCommands};
        private static readonly List<List<string>> CompoundCommands = new List<List<string>>{MovementCommands, TurnCommands, PingCommands, TransferCommands, OffCommands, OnCommands};

        public static Command GetCommand(string phrase) {
            Command c = new Command();
            
            // Check if the phrase contains a command that requires more info
            foreach (List<string> commandList in CompoundCommands) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                        if (!c.IsValid()) c = GetCompoundCommand(phrase, commandList);
                    }
                }
            }

            // If no compound command is found check for single word commands
            // Have to do it in this order because of the "go" command being used for movement and speed
            if (!c.IsValid()) {
                // Check if the phrase contains a single word command
                foreach (List<string> commandList in SingleCommands) {
                    foreach (string command in commandList) {
                        if (phrase.Contains(command)) {
                            if (commandList == SpeedCommands) return new SpeedCommand(command);
                            if (commandList == ShootCommands) return new Command(Command.CommandType.Shoot);
                        }
                    }
                }
            }

            return c;
        }
        
        private static Command GetCompoundCommand(string phrase, List<string> commandList) {
            if (commandList.Equals(MovementCommands)) return GetMovementCommand(phrase);
            if (commandList.Equals(TurnCommands)) return GetTurnCommand(phrase);
            if (commandList.Equals(PingCommands)) return GetPingCommand(phrase);
            if (commandList.Equals(TransferCommands)) return GetTransferCommand(phrase);
            if (commandList.Equals(OffCommands)) return GetToggleCommand(phrase, false);
            if (commandList.Equals(OnCommands)) return GetToggleCommand(phrase, true);

            return new Command(); // Return an invalid command
        }
        
        private static Command GetMovementCommand(string phrase) {
            string data = GetDirection(phrase);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Direction, data);

            data = GetDestination(phrase);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Destination, data);

            data = GetGridCoord(phrase);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Grid, data);

            return new Command(); // Return an invalid command
        }
        
        private static Command GetTurnCommand(string phrase) {
            string data = GetDirection(phrase);
            if (data != null) return new TurnCommand(TurnCommand.TurnType.Direction, data);

            data = GetDestination(phrase);
            if (data != null) return new TurnCommand(TurnCommand.TurnType.Destination, data);

            data = GetGridCoord(phrase);
            if (data != null) return new TurnCommand(TurnCommand.TurnType.Grid, data);

            return new Command(); // Return an invalid command
        }

        private static Command GetPingCommand(string phrase) {
            string pingType = GetPingType(phrase);
            string gridCoord = GetGridCoord(phrase);

            if (pingType != null && gridCoord != null) return new PingCommand(pingType, gridCoord);
            return new Command(); // Return an invalid command
        }

        private static Command GetTransferCommand(string phrase) {
            if (HasTransferableObject(phrase)) return new Command(Command.CommandType.Transfer);
            return new Command(); // Return an invalid command
        }

        private static Command GetToggleCommand(string phrase, bool on) { // on is true if turning on, false if turning off
            string activatableObject = GetActivatableObject(phrase);

            if (LockCommands.Contains(activatableObject)) {
                string lockTarget = GetLockTarget(phrase);
                // Only need a target if lock is being enabled
                if (lockTarget != null || !on) return new ToggleCommand(on, ToggleCommand.ObjectType.Lock, lockTarget);
            }

            if (activatableObject != null) return new ToggleCommand(on, ToggleCommand.ObjectType.MiningLaser);
            
            return new Command(); // Return an invalid command 
        }

        private static string GetDirection(string phrase) {
            foreach (List<string> commandList in Directions) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) return command;
                }
            }
    
            return null;
        }
        
        private static string GetDestination(string phrase) {
            foreach (string command in Destinations) {
                if (phrase.Contains(command)) return command;
            }

            return null;
        }
        
        private static string GetGridCoord(string phrase) {
            Match coordMatch = Regex.Match(phrase, GridCoordRegex);
            if (!coordMatch.Success) return null;
            return coordMatch.Value;
        }

        private static string GetPingType(string phrase) {
            foreach (string command in PingTypes) {
                if (phrase.Contains(command)) return command;
            }

            return null;
        }
        
        private static string GetActivatableObject(string phrase) {
            foreach (List<string> commandList in Activatable) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                        return command;
                    }
                }
            }
    
            return null;
        }
        
        private static string GetLockTarget(string phrase) {
            foreach (string command in LockTargets) {
                if (phrase.Contains(command)) return command;
            }

            return null;
        }

        private static bool HasTransferableObject(string phrase) {
            foreach (string command in Transferable) {
                if (phrase.Contains(command)) return true;
            }

            return false;
        }

    }
}