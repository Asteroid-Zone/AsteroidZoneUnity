using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon.StructWrapping;
using PlayGame.Speech.Commands;
using UnityEngine;

namespace PlayGame.Speech {
    public static class Grammar {
        
        private const string GridCoordRegex = @"[a-z]( )?(\d+)";
        
        // Lists containing synonyms
        private static readonly List<string> Pirate = new List<string>{"pirate", "enemy"};
        private static readonly List<string> Asteroid = new List<string>{"asteroid", "meteor"};
        private static readonly List<string> MiningLaser = new List<string>{"mining laser", "laser", "mining beam"};
        private static readonly List<string> SpaceStation = new List<string>{"station", "space station", "base"};
        private static readonly List<string> Ping = new List<string>{"ping", "pin", "mark"};
        private static readonly List<string> Resources = new List<string>{"resources", "materials"};

        private static readonly List<string> MovementCommands = new List<string>{"move", "go"};
        private static readonly List<string> TurnCommands = new List<string>{"face", "turn"};
        
        // Todo add left/right commands
        private static readonly List<string> CompassDirections = new List<string>{"north", "east", "south", "west"};
        private static readonly List<List<string>> Directions = new List<List<string>>{CompassDirections};
        
        private static readonly List<List<string>> Destinations = new List<List<string>>{SpaceStation, Ping};
        
        private static readonly List<List<string>> PingTypes = new List<List<string>>{Asteroid, Pirate};

        private static readonly List<string> SpeedCommands = new List<string>{"stop", "go"};
        
        private static readonly List<string> TransferCommands = new List<string>{"transfer", "deposit"};

        private static readonly List<string> LockCommands = new List<string>{"lock", "aim"};
        private static readonly List<List<string>> LockTargets = new List<List<string>>{Pirate, Asteroid};

        private static readonly List<string> OnCommands = new List<string>{"activate", "engage", "turn on", "lock"};
        private static readonly List<string> OffCommands = new List<string>{"deactivate", "disengage", "turn off"};
        private static readonly List<List<string>> Activatable = new List<List<string>>{MiningLaser, LockCommands};
        
        private static readonly List<string> ShootCommands = new List<string>{"shoot", "fire"};

        private static readonly List<List<string>> SingleCommands = new List<List<string>>{SpeedCommands, ShootCommands};
        private static readonly List<List<string>> CompoundCommands = new List<List<string>>{MovementCommands, TurnCommands, Ping, TransferCommands, OffCommands, OnCommands};

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
            if (commandList.Equals(Ping)) return GetPingCommand(phrase);
            if (commandList.Equals(TransferCommands)) return GetTransferCommand(phrase);
            if (commandList.Equals(OffCommands)) return GetToggleCommand(phrase, false);
            if (commandList.Equals(OnCommands)) return GetToggleCommand(phrase, true);

            return new Command(); // Return an invalid command
        }
        
        private static Command GetMovementCommand(string phrase) {
            string data = GetDirection(phrase);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Direction, data);

            data = GetCommandListIdentifier(phrase, Destinations);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Destination, data);

            data = GetGridCoord(phrase);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Grid, data);

            return new Command(); // Return an invalid command
        }
        
        private static Command GetTurnCommand(string phrase) {
            string data = GetDirection(phrase);
            if (data != null) return new TurnCommand(TurnCommand.TurnType.Direction, data);

            data = GetCommandListIdentifier(phrase, Destinations);
            if (data != null) return new TurnCommand(TurnCommand.TurnType.Destination, data);

            data = GetGridCoord(phrase);
            if (data != null) return new TurnCommand(TurnCommand.TurnType.Grid, data);

            return new Command(); // Return an invalid command
        }

        private static Command GetPingCommand(string phrase) {
            string pingType = GetCommandListIdentifier(phrase, PingTypes);
            string gridCoord = GetGridCoord(phrase);

            if (pingType != null && gridCoord != null) return new PingCommand(pingType, gridCoord);
            return new Command(); // Return an invalid command
        }

        private static Command GetTransferCommand(string phrase) {
            if (GetCommandFromList(phrase, Resources) != null) return new Command(Command.CommandType.Transfer);
            return new Command(); // Return an invalid command
        }

        private static Command GetToggleCommand(string phrase, bool on) { // on is true if turning on, false if turning off
            List<string> activatableObject = GetCommandList(phrase, Activatable);

            // If toggling lock
            if (activatableObject.Equals(LockCommands)) {
                string lockTarget = GetCommandListIdentifier(phrase, LockTargets);
                // Only need a target if lock is being enabled
                if (lockTarget != null || !on) return new ToggleCommand(on, ToggleCommand.ObjectType.Lock, lockTarget);
            }

            if (activatableObject.Equals(MiningLaser)) return new ToggleCommand(on, ToggleCommand.ObjectType.MiningLaser);
            
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
        
        private static string GetGridCoord(string phrase) {
            Match coordMatch = Regex.Match(phrase, GridCoordRegex);
            if (!coordMatch.Success) return null;
            return coordMatch.Value;
        }

        // Returns the first element of the list which contains the string which was found in the phrase or null
        public static string GetCommandListIdentifier(string phrase, List<List<string>> SearchList) {
            List<string> commandList = GetCommandList(phrase, SearchList);
            if (commandList != null) return commandList[0];
            return null;
        }
        
        // Returns the list which contains the string which was found in the phrase or null
        private static List<string> GetCommandList(string phrase, List<List<string>> SearchList) {
            foreach (List<string> commandList in SearchList) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) return commandList;
                }
            }

            return null;
        }

        // Returns the string which was found in the phrase or null
        private static string GetCommandFromList(string phrase, List<string> SearchList) {
            foreach (string command in SearchList) {
                if (phrase.Contains(command)) return command;
            }

            return null;
        }
        
    }
}