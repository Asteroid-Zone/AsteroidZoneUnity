using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayGame.Speech.Commands;

namespace PlayGame.Speech {
    public static class Grammar {
        
        private const string GridCoordRegex = @"[a-z]( )?(\d+)";

        private static readonly List<string> MovementCommands = new List<string>{"move", "go"};
        private static readonly List<string> TurnCommands = new List<string>{"face", "turn"};
        
        // Todo add left/right commands
        private static readonly List<string> CompassDirections = new List<string>{"north", "east", "south", "west"};
        private static readonly List<List<string>> Directions = new List<List<string>>{CompassDirections};
        
        private static readonly List<string> Destinations = new List<string>{"space station", "station", "ping"};
        
        private static readonly List<string> PingTypes = new List<string>{"none", "asteroid", "pirate"};
        private static readonly List<string> PingCommands = new List<string>{"ping", "pin", "mark"};

        private static readonly List<string> SpeedCommands = new List<string>{"stop", "go"};
        
        private static readonly List<string> TransferCommands = new List<string>{"transfer", "deposit"};
        private static readonly List<string> Transferable = new List<string>{"resources", "materials"};

        private static readonly List<string> MiningLaser = new List<string>{"mining laser", "laser", "mining beam"};
        
        private static readonly List<string> LockCommands = new List<string>{"lock", "aim"};
        private static readonly List<string> LockTargets = new List<string>{"pirate", "asteroid"};
        
        private static readonly List<string> OnCommands = new List<string>{"activate", "engage", "turn on"};
        private static readonly List<string> OffCommands = new List<string>{"deactivate", "disengage", "turn off"};
        private static readonly List<List<string>> Activatable = new List<List<string>>{MiningLaser, LockCommands};
        
        private static readonly List<string> ShootCommands = new List<string>{"shoot", "fire"};

        private static readonly List<List<string>> SingleCommands = new List<List<string>>{SpeedCommands, ShootCommands};
        private static readonly List<List<string>> CompoundCommands = new List<List<string>>{MovementCommands, TurnCommands, PingCommands, TransferCommands, OnCommands, OffCommands};

        public static Command GetCommand(string phrase) {
            // Check if the phrase contains a command that requires more info
            foreach (List<string> commandList in CompoundCommands) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                       return GetCompoundCommand(phrase, commandList);
                    }
                }
            }
            
            // Check if the phrase contains a single word command
            foreach (List<string> commandList in SingleCommands) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                        if (commandList == SpeedCommands) return new Command(); // todo create speed command
                        if (commandList == ShootCommands) return new Command(); // todo create shoot command
                    }
                }
            }

            return new Command(); // Return an invalid command
        }
        
        private static Command GetCompoundCommand(string phrase, List<string> commandList) {
            if (commandList.Equals(MovementCommands)) return GetMovementCommand(phrase);
            if (commandList.Equals(TurnCommands)) return GetTurnCommand(phrase);
            if (commandList.Equals(PingCommands)) return GetPingCommand(phrase);
            if (commandList.Equals(TransferCommands)) return GetTransferCommand(phrase);
            if (commandList.Equals(OnCommands)) return GetTurnOnCommand(phrase);
            // todo if (commandList.Equals(OffCommands)) return HasActivatableObject(phrase);

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

        private static Command GetTurnOnCommand(string phrase) {
            string activatableObject = GetActivatableObject(phrase);

            if (LockCommands.Contains(activatableObject)) {
                string lockTarget = GetLockTarget(phrase);
                if (lockTarget != null) return new TurnOnCommand(TurnOnCommand.ObjectType.Lock, lockTarget);
            }

            if (activatableObject != null) return new TurnOnCommand(TurnOnCommand.ObjectType.MiningLaser);
            
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
        
        // Checks if a phrase is valid
        public static bool IsValidPhrase(string phrase) {
            // Check if the phrase contains a command that requires more info
            foreach (List<string> commandList in CompoundCommands) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                        if (HasValidSubject(phrase, commandList)) return true;
                    }
                }
            }
            
            // Check if the phrase contains a single word command
            foreach (List<string> commandList in SingleCommands) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) return true;
                }
            }

            return false;
        }

        // Calls the correct method to determine if the command has the required information
        private static bool HasValidSubject(string phrase, List<string> commandList) {
            if (commandList.Equals(MovementCommands)) return HasDirection(phrase) || HasDestination(phrase);
            if (commandList.Equals(TurnCommands)) return HasDirection(phrase);
            if (commandList.Equals(PingCommands)) return HasPingType(phrase) && HasGridCoord(phrase);
            if (commandList.Equals(TransferCommands)) return HasTransferableObject(phrase);
            if (commandList.Equals(OnCommands)) return HasActivatableObject(phrase);
            if (commandList.Equals(OffCommands)) return HasActivatableObject(phrase);

            return false;
        }

        private static bool HasDirection(string phrase) {
            foreach (List<string> commandList in Directions) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) return true;
                }
            }
    
            return false;
        }
        
        private static bool HasDestination(string phrase) {
            foreach (string command in Destinations) {
                if (phrase.Contains(command)) return true;
            }

            if (HasGridCoord(phrase)) return true;
    
            return false;
        }

        private static bool HasGridCoord(string phrase) {
            Match coordMatch = Regex.Match(phrase, GridCoordRegex);
            return coordMatch.Success;
        }
        
        private static bool HasPingType(string phrase) {
            foreach (string command in PingTypes) {
                if (phrase.Contains(command)) return true;
            }

            return false;
        }
        
        private static bool HasTransferableObject(string phrase) {
            foreach (string command in Transferable) {
                if (phrase.Contains(command)) return true;
            }

            return false;
        }

        private static bool HasActivatableObject(string phrase) {
            foreach (List<string> commandList in Activatable) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                        if (commandList == LockCommands) return HasLockTarget(phrase); // Lock on commands need a target
                        return true;
                    }
                }
            }
    
            return false;
        }

        private static bool HasLockTarget(string phrase) {
            foreach (string command in LockTargets) {
                if (phrase.Contains(command)) return true;
            }

            return false;
        }

    }
}