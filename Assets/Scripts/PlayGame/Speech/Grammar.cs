using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayGame.Speech.Commands;
using Statics;

namespace PlayGame.Speech {
    public static class Grammar {
        
        private const string GridCoordRegex = @"[a-z]( )?(\d+)";
        
        // Lists containing synonyms for commands
        private static readonly List<string> MovementCommands = new List<string>{"move", "go"};
        private static readonly List<string> InstantTurn = new List<string>{"face", "look"};
        private static readonly List<string> SmoothTurn = new List<string>{"turn"};
        private static readonly List<string> TurnCommands = new List<string>(); // Initialised at startup
        private static readonly List<string> SpeedCommands = new List<string>{Strings.Stop, Strings.Go};
        private static readonly List<string> TransferCommands = new List<string>{"transfer", "deposit"};
        
        private static readonly List<string> GenericOnCommands = new List<string>{"activate", "engage", "turn on"}; // Can be used to activate anything
        private static readonly List<string> LockCommands = new List<string>{"lock", "aim", "target"}; // Can only be used to activate a lock
        private static readonly List<string> ShootCommands = new List<string>{"shoot", "fire"}; // Can only be used to activate laser gun
        //private static readonly List<string> OnCommands = new List<string>{GenericOnCommands, LockCommands, ShootCommands};
        private static readonly List<string> OnCommands = new List<string>(); // Is initialised to the above line at startup
        private static readonly List<string> OffCommands = new List<string>{"deactivate", "disengage", "turn off", "stop"};

        // Lists containing synonyms for objects
        private static readonly List<string> Pirate = new List<string>{Strings.Pirate, "enemy"};
        private static readonly List<string> Asteroid = new List<string>{Strings.Asteroid, "meteor"};
        private static readonly List<string> MiningLaser = new List<string>{Strings.MiningLaser, "mining beam"};
        private static readonly List<string> SpaceStation = new List<string>{Strings.SpaceStation, "space station", "base"};
        private static readonly List<string> Ping = new List<string>{Strings.Ping, "pin", "mark"};
        private static readonly List<string> Resources = new List<string>{Strings.Resources, "materials"};
        private static readonly List<string> LaserGun = new List<string> {Strings.LaserGun, "gun", "shoot"};
        
        private static readonly List<string> CompassDirections = new List<string>{Strings.North, Strings.East, Strings.South, Strings.West};
        private static readonly List<string> RelativeDirections = new List<string>{Strings.Forward, Strings.Back, Strings.Left, Strings.Right};
        private static readonly List<List<string>> Directions = new List<List<string>>{CompassDirections, RelativeDirections};
        
        private static readonly List<List<string>> Destinations = new List<List<string>>{SpaceStation, Ping, Pirate, Asteroid};
        private static readonly List<List<string>> PingTypes = new List<List<string>>{Asteroid, Pirate};
        private static readonly List<List<string>> LockTargets = new List<List<string>>{Pirate, Asteroid};
        private static readonly List<List<string>> Activatable = new List<List<string>>{MiningLaser, LockCommands, LaserGun};
        
        private static readonly List<List<string>> SingleCommands = new List<List<string>>{SpeedCommands, ShootCommands};
        private static readonly List<List<string>> CompoundCommands = new List<List<string>>{MovementCommands, TurnCommands, Ping, TransferCommands, OffCommands, OnCommands};

        private static readonly List<string> CommandWords;
        
        static Grammar() {
            TurnCommands.AddRange(InstantTurn);
            TurnCommands.AddRange(SmoothTurn);
            
            OnCommands.AddRange(GenericOnCommands);
            OnCommands.AddRange(LockCommands);
            OnCommands.AddRange(ShootCommands);

            CommandWords = GetAllCommandWords();
        }

        private static List<string> GetAllCommandWords() {
            List<string> commandWords = new List<string>();
            
            foreach (List<string> commandList in SingleCommands) {
                commandWords.AddRange(commandList);
            }

            foreach (List<string> commandList in CompoundCommands) {
                commandWords.AddRange(commandList);
            }

            return commandWords;
        }

        // Returns the command which was most likely intended by the player
        private static string GetSuggestedCommand(string phrase) {
            string closestCommandWord = GetClosestWordFromList(CommandWords, phrase.Split(' ')[0]);

            return closestCommandWord;
        }

        private static string GetClosestWordFromList(List<string> list, string word) {
            string closestWord = "";
            int distance = word.Length;
            
            foreach (string word1 in list) {
                int d = GetLevenshteinDistance(word1, word);
                if (d < distance) {
                    distance = d;
                    closestWord = word1;
                }
            }

            return closestWord;
        }

        // Returns the Levenshtein distance (number of single character edits required to make the strings equal)
        private static int GetLevenshteinDistance(string a, string b) {
            if (string.IsNullOrEmpty(a)) {
                if (!string.IsNullOrEmpty(b)) return b.Length;
                return 0;
            }

            if (string.IsNullOrEmpty(b)) {
                if (!string.IsNullOrEmpty(a)) return a.Length;
                return 0;
            }

            int cost;
            int[,] d = new int[a.Length, b.Length];
            int min1;
            int min2;
            int min3;
            
            for (int i = 0; i <= d.GetUpperBound(0); i += 1) {
                d[i, 0] = i;
            }

            for (int i = 0; i <= d.GetUpperBound(1); i += 1) {
                d[0, i] = i;
            }
            
            for (int i = 1; i <= d.GetUpperBound(0); i += 1) {
                for (int j = 1; j <= d.GetUpperBound(1); j += 1) {
                    cost = (a[i-1] != b[j-1])? 1 : 0; 

                    min1 = d[i - 1, j] + 1;
                    min2 = d[i, j - 1] + 1;
                    min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            
            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }

        public static Command GetCommand(string phrase) {
            Command c = new Command();
            
            // Check if the phrase contains a command that requires more info
            foreach (List<string> commandList in CompoundCommands) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                        if (!c.IsValid()) c = GetCompoundCommand(phrase, commandList, command);
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
                            if (commandList == ShootCommands) return new ToggleCommand(true, ToggleCommand.ObjectType.LaserGun); // Turn laser gun on
                        }
                    }
                }
            }

            return c;
        }
        
        private static Command GetCompoundCommand(string phrase, List<string> commandList, string command) {
            if (commandList.Equals(MovementCommands)) return GetMovementCommand(phrase);
            if (commandList.Equals(TurnCommands)) return GetTurnCommand(phrase, command);
            if (commandList.Equals(Ping)) return GetPingCommand(phrase);
            if (commandList.Equals(TransferCommands)) return GetTransferCommand(phrase);
            if (commandList.Equals(OffCommands)) return GetToggleCommand(phrase, false, command);
            if (commandList.Equals(OnCommands)) return GetToggleCommand(phrase, true, command);

            return new Command(); // Return an invalid command
        }
        
        private static Command GetMovementCommand(string phrase) {
            string data = GetDirection(phrase);
            if (data != null) {
                // If moving using relative direction dont turn the camera
                if (RelativeDirections.Contains(data)) {
                    return new MovementCommand(MovementCommand.MovementType.Direction, data, false, MovementCommand.TurnType.None);
                }
                return new MovementCommand(MovementCommand.MovementType.Direction, data, false, MovementCommand.TurnType.Instant);
            }

            data = GetCommandListIdentifier(phrase, Destinations);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Destination, data, false, MovementCommand.TurnType.Instant);

            data = GetGridCoord(phrase);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Grid, data, false, MovementCommand.TurnType.Instant);

            return new Command(); // Return an invalid command
        }
        
        private static Command GetTurnCommand(string phrase, string command) {
            string data = GetDirection(phrase);
            if (data != null) {
                if (InstantTurn.Contains(command)) {
                    return new MovementCommand(MovementCommand.MovementType.Direction, data, true, MovementCommand.TurnType.Instant);
                }
                
                if (SmoothTurn.Contains(command)) {
                    if (data == Strings.Left || data == Strings.Right) {
                        return new MovementCommand(MovementCommand.MovementType.Direction, data, true, MovementCommand.TurnType.Smooth);
                    }
                    return new MovementCommand(MovementCommand.MovementType.Direction, data, true, MovementCommand.TurnType.Instant);
                }
            }

            data = GetCommandListIdentifier(phrase, Destinations);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Destination, data, true, MovementCommand.TurnType.Instant);

            data = GetGridCoord(phrase);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Grid, data, true, MovementCommand.TurnType.Instant);

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

        private static Command GetToggleCommand(string phrase, bool on, string command) { // on is true if turning on, false if turning off
            List<string> activatableObject = GetCommandList(phrase, Activatable);
            if (activatableObject == null) return new Command(); // Return an invalid command
            
            // If toggling lock
            if (activatableObject.Equals(LockCommands) && (LockCommands.Contains(command) || GenericOnCommands.Contains(command) || OffCommands.Contains(command))) { // Has to use either a lock command or a generic on command
                string lockTarget = GetCommandListIdentifier(phrase, LockTargets);
                // Only need a target if lock is being enabled
                if (lockTarget != null || !on) return new ToggleCommand(on, ToggleCommand.ObjectType.Lock, lockTarget);
            }

            // Must use either generic or shoot command
            if (activatableObject.Equals(MiningLaser) && (GenericOnCommands.Contains(command) || OffCommands.Contains(command) || ShootCommands.Contains(command))) return new ToggleCommand(on, ToggleCommand.ObjectType.MiningLaser);
            if (activatableObject.Equals(LaserGun) && (GenericOnCommands.Contains(command) || OffCommands.Contains(command) || ShootCommands.Contains(command))) return new ToggleCommand(on, ToggleCommand.ObjectType.LaserGun);
            
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
        public static string GetCommandListIdentifier(string phrase, List<List<string>> searchList) {
            List<string> commandList = GetCommandList(phrase, searchList);
            if (commandList != null) return commandList[0];
            return null;
        }
        
        // Returns the list which contains the string which was found in the phrase or null
        private static List<string> GetCommandList(string phrase, List<List<string>> searchList) {
            foreach (List<string> commandList in searchList) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) return commandList;
                }
            }

            return null;
        }

        // Returns the string which was found in the phrase or null
        private static string GetCommandFromList(string phrase, List<string> searchList) {
            foreach (string command in searchList) {
                if (phrase.Contains(command)) return command;
            }

            return null;
        }
        
    }
}