using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayGame.Player;
using PlayGame.Speech.Commands;
using Statics;
using UnityEngine;

namespace PlayGame.Speech {
    public static class Grammar {

        // Stores information about the data in the phrase
        private struct DataProvided {
            public string direction;
            public string destination;
            public string grid;
            public string pingType;
            public string resource;
            public string activatableObject;
            public string lockTarget;
            public string stationModule;
        }
        
        private const string GridCoordRegex = @"[a-z]( )?(\d+)";
        
        // Lists containing synonyms for commands
        private static readonly List<string> MovementCommands = new List<string>{"move", "go"}; // Needs direction/destination/grid
        private static readonly List<string> InstantTurn = new List<string>{"face", "look"};
        private static readonly List<string> SmoothTurn = new List<string>{"turn"};
        private static readonly List<string> TurnCommands = new List<string>(); // Initialised at startup, Needs direction/destination/grid
        private static readonly List<string> SpeedCommands = new List<string>{Strings.Stop, Strings.Go};
        private static readonly List<string> TransferCommands = new List<string>{"transfer", "deposit"}; // Needs resources
        
        private static readonly List<string> GenericOnCommands = new List<string>{"activate", "engage", "turn on"}; // Can be used to activate anything
        private static readonly List<string> LockCommands = new List<string>{"lock", "aim", "target"}; // Can only be used to activate a lock
        private static readonly List<string> ShootCommands = new List<string>{"shoot", "fire"}; // Can only be used to activate laser gun or mining laser
        //private static readonly List<string> OnCommands = new List<string>{GenericOnCommands, LockCommands, ShootCommands};
        private static readonly List<string> OnCommands = new List<string>(); // Is initialised to the above line at startup
        private static readonly List<string> OffCommands = new List<string>{"deactivate", "disengage", "turn off", "stop"};
        private static readonly List<string> RepairCommands = new List<string>{"repair", "fix"};

        // Lists containing synonyms for objects
        private static readonly List<string> Pirate = new List<string>{Strings.Pirate, "enemy"};
        private static readonly List<string> Asteroid = new List<string>{Strings.Asteroid, "meteor"};
        private static readonly List<string> MiningLaser = new List<string>{Strings.MiningLaser, "mining beam"};
        private static readonly List<string> SpaceStation = new List<string>{Strings.SpaceStation, "space station", "base"};
        private static readonly List<string> Ping = new List<string>{Strings.Ping, "pin", "mark"};
        private static readonly List<string> Resources = new List<string>{Strings.Resources, "materials"};
        private static readonly List<string> LaserGun = new List<string> {Strings.LaserGun, "gun", "shoot"};
        
        private static readonly List<string> Hyperdrive = new List<string> {Strings.Hyperdrive, "warp drive", "hyper drive"};
        private static readonly List<string> Hull = new List<string> {Strings.Hull};
        private static readonly List<string> ShieldGenerator = new List<string> {Strings.ShieldGenerator, "shield"};
        
        private static readonly List<string> CompassDirections = new List<string>{Strings.North, Strings.East, Strings.South, Strings.West};
        private static readonly List<string> RelativeDirections = new List<string>{Strings.Forward, Strings.Back, Strings.Left, Strings.Right};
        private static readonly List<List<string>> Directions = new List<List<string>>{CompassDirections, RelativeDirections};
        
        private static readonly List<List<string>> Destinations = new List<List<string>>{SpaceStation, Ping, Pirate, Asteroid};
        private static readonly List<List<string>> PingTypes = new List<List<string>>{Asteroid, Pirate};
        private static readonly List<List<string>> LockTargets = new List<List<string>>{Pirate, Asteroid};
        private static readonly List<List<string>> Activatable = new List<List<string>>{MiningLaser, LockCommands, LaserGun, Hyperdrive};
        private static readonly List<List<string>> GenericActivatableObjects = new List<List<string>>{Hyperdrive}; // Objects that are only activated using the generic on commands and don't need any extra data
        private static readonly List<List<string>> StationModules = new List<List<string>>{Hyperdrive, Hull, ShieldGenerator};
        
        private static readonly List<List<string>> SingleCommands = new List<List<string>>{SpeedCommands, ShootCommands};
        private static readonly List<List<string>> CompoundCommands = new List<List<string>>{MovementCommands, TurnCommands, Ping, TransferCommands, OffCommands, OnCommands, RepairCommands};

        private static readonly List<string> CommandWords;
        
        static Grammar() {
            TurnCommands.AddRange(InstantTurn);
            TurnCommands.AddRange(SmoothTurn);
            
            OnCommands.AddRange(GenericOnCommands);
            OnCommands.AddRange(LockCommands);
            OnCommands.AddRange(ShootCommands);

            CommandWords = GetAllCommandWords();
        }

        // Returns a list containing all the command words
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
        public static string GetSuggestedCommand(string phrase) {
            // Find commands that have some of the required data in the phrase
            List<Tuple<string, float>> partiallyCompleteCommands = GetPartiallyCompleteCommands(phrase); 
            Tuple<string, float> mostCompleteCommand = new Tuple<string, float>("", 0);
            
            if (partiallyCompleteCommands.Count != 0) {
                foreach (Tuple<string, float> command in partiallyCompleteCommands) {
                    if (command.Item2 > mostCompleteCommand.Item2) mostCompleteCommand = command;
                }
            }
            
            if (mostCompleteCommand.Item2 > 0) return mostCompleteCommand.Item1;
            
            // If no partially complete commands are found search for the closest command word using levenshtein distance
            Tuple<string, int> closestCommandWord = GetClosestCommand(phrase);
            
            int thresholdDistance = closestCommandWord.Item1.Length / 2; // More than half of the letters in the command must be correct
            if (closestCommandWord.Item1 != "" && closestCommandWord.Item2 < thresholdDistance) return closestCommandWord.Item1;
            return Strings.NoCommand;
        }

        private static List<Tuple<string, float>> GetPartiallyCompleteCommands(string phrase) {
            DataProvided dataProvided = GetDataProvided(phrase);
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)

            commands.AddRange(GetPartiallyCompleteMovementCommands(phrase, dataProvided, MovementCommands));
            commands.AddRange(GetPartiallyCompleteMovementCommands(phrase, dataProvided, TurnCommands));
            
            commands.Add(GetPartiallyCompletePingCommand(phrase, dataProvided));
            commands.Add(GetPartiallyCompleteTransferCommand(phrase, dataProvided));
            commands.Add(GetPartiallyCompleteOffCommand(phrase, dataProvided));
            commands.AddRange(GetPartiallyCompleteRepairCommands(phrase, dataProvided));
            
            commands.AddRange(GetPartiallyCompleteOnCommands(phrase, dataProvided));

            return commands;
        }

        private static List<Tuple<string, float>> GetPartiallyCompleteRepairCommands(string phrase, DataProvided dataProvided) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)
            
            float completeness = 0; // Completeness of command
            float completenessAmount = 0; // Completeness of command with repair amount
            float third = 1f / 3;

            string c = "";
            
            // Add the command word they used or the default repair command if one isn't found
            foreach (string command in RepairCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    completenessAmount = third;
                    c = command;
                }
            }
            if (c == "") c = RepairCommands[0];
            
            // Station module
            if (dataProvided.stationModule != null) {
                completeness += 0.5f;
                completenessAmount += third;
                c += " " + dataProvided.stationModule;
            } else {
                c += " (station module)";
            }

            commands.Add(new Tuple<string, float>(c, completeness)); // Command without repair amount
            
            // Repair amount
            int? repairAmount = GetNumber(phrase);
            if (repairAmount != null) {
                completenessAmount += third;
                c += " " + repairAmount;
            } else {
                c += " (repair amount)";
            }

            commands.Add(new Tuple<string, float>(c, completenessAmount)); // Command with repair amount

            return commands;
        }

        private static Tuple<string, float> GetPartiallyCompleteLockCommand(string phrase, DataProvided dataProvided) {
            float completeness = 0;

            string c = "";
            
            // Add the command word they used or the default lock command if one isn't found
            foreach (string command in LockCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    c = command;
                }
            }
            if (c == "") c = LockCommands[0];
            
            // Lock target
            if (dataProvided.lockTarget != null) {
                completeness += 0.5f;
                c += " " + dataProvided.lockTarget;
            } else {
                c += " (lock target)";
            }

            return new Tuple<string, float>(c, completeness);
        }
        
        private static List<Tuple<string, float>> GetPartiallyCompleteLaserCommands(string phrase, DataProvided dataProvided) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)
            float completeness = 0;

            string c = "";
            
            // Add the command word they used or the default on command if one isn't found
            foreach (string command in ShootCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    c = command;
                }
            }
            if (c == "") { // If no shoot command was found check for generic on commands
                foreach (string command in GenericOnCommands) {
                    if (phrase.Contains(command)) {
                        completeness = 0.5f;
                        c = command;
                    }
                }
                if (c == "") c = GenericOnCommands[0];
            }
            
            // Laser
            if (dataProvided.activatableObject != null) {
                if (MiningLaser.Contains(dataProvided.activatableObject)) {
                    commands.Add(new Tuple<string, float>(c + " " + dataProvided.activatableObject, completeness + 0.5f));
                }
                
                if (LaserGun.Contains(dataProvided.activatableObject)) {
                    commands.Add(new Tuple<string, float>(c + " " + dataProvided.activatableObject, completeness + 0.5f));
                }
            } else {
                if (completeness != 0) {
                    commands.Add(new Tuple<string, float>(c + " " + MiningLaser[0], completeness));
                    commands.Add(new Tuple<string, float>(c + " " + LaserGun[0], completeness));
                }
            }
            
            return commands;
        }

        private static List<Tuple<string, float>> GetPartiallyCompleteGenericOnCommands(string phrase, DataProvided dataProvided) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)
            float completeness = 0;

            string c = "";
            
            // Add the command word they used or the default on command if one isn't found
            foreach (string command in GenericOnCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    c = command;
                }
            }
            if (c == "") c = GenericOnCommands[0];
            
            // Activatable Object
            if (dataProvided.activatableObject != null) {
                foreach (List<string> commandList in GenericActivatableObjects) {
                    if (commandList.Contains(dataProvided.activatableObject)) {
                        commands.Add(new Tuple<string, float>(c + " " + dataProvided.activatableObject, completeness + 0.5f));
                    }
                }
            } else {
                if (completeness != 0) {
                    foreach (List<string> commandList in GenericActivatableObjects) {
                        commands.Add(new Tuple<string, float>(c + " " + commandList[0], completeness));
                    }
                }
            }
            
            return commands;
        }
        
        private static List<Tuple<string, float>> GetPartiallyCompleteOnCommands(string phrase, DataProvided dataProvided) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)

            commands.Add(GetPartiallyCompleteLockCommand(phrase, dataProvided));
            commands.AddRange(GetPartiallyCompleteLaserCommands(phrase, dataProvided));
            commands.AddRange(GetPartiallyCompleteGenericOnCommands(phrase, dataProvided));

            return commands;
        }
        
        // Returns a transfer command with the percentage of data provided
        private static Tuple<string, float> GetPartiallyCompleteOffCommand(string phrase, DataProvided dataProvided) {
            float completeness = 0;

            string c = "";
            
            // Add the command word they used or the default off command if one isn't found
            foreach (string command in OffCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    c = command;
                }
            }
            if (c == "") c = OffCommands[0];
            
            // Activatable Object
            if (dataProvided.activatableObject != null) {
                completeness += 0.5f;
                c += " " + dataProvided.activatableObject;
            } else {
                c += " (activatable object)";
            }

            return new Tuple<string, float>(c, completeness);
        }
        
        // Returns a transfer command with the percentage of data provided
        private static Tuple<string, float> GetPartiallyCompleteTransferCommand(string phrase, DataProvided dataProvided) {
            float completeness = 0;

            string c = "";
            
            // Add the command word they used or the default transfer command if one isn't found
            foreach (string command in TransferCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    c = command;
                }
            }
            if (c == "") c = TransferCommands[0];
            
            // Resource
            if (dataProvided.resource != null) {
                completeness += 0.5f;
                c += " " + dataProvided.resource;
            } else {
                c += " " + Resources[0];
            }

            return new Tuple<string, float>(c, completeness);
        }

        // Returns a ping command with the percentage of data provided
        private static Tuple<string, float> GetPartiallyCompletePingCommand(string phrase, DataProvided dataProvided) {
            float completeness = 0;
            float third = 1f / 3;

            string c = "";
            
            // Add the command word they used or the default ping command if one isn't found
            foreach (string command in Ping) {
                if (phrase.Contains(command)) {
                    completeness = third;
                    c = command;
                }
            }
            if (c == "") c = Ping[0];
            
            // Ping type
            if (dataProvided.pingType != null) {
                completeness += third;
                c += " " + dataProvided.pingType;
            } else {
                c += " (ping type)";
            }

            // Grid coord
            if (dataProvided.grid != null) {
                completeness += third;
                c += " " + dataProvided.grid;
            } else {
                c += " (grid coord)";
            }

            return new Tuple<string, float>(c, completeness);
        }

        private static List<Tuple<string, float>> GetPartiallyCompleteMovementCommands(string phrase, DataProvided dataProvided, List<string> commandList) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)

            float completeness = 0;
            string c = "";
            
            // Check for command word
            foreach (string command in commandList) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    c = command;
                }
            }
            if (c == "") c = commandList[0];

            if (dataProvided.direction != null) {
                commands.Add(new Tuple<string, float>(c + " " + dataProvided.direction, completeness + 0.5f));
            } else {
                if (completeness != 0) commands.Add(new Tuple<string, float>(c + " (direction)", completeness));
            }
            
            if (dataProvided.destination != null) {
                commands.Add(new Tuple<string, float>(c + " " + dataProvided.destination, completeness + 0.5f));
            } else {
                if (completeness != 0) commands.Add(new Tuple<string, float>(c + " (destination)", completeness));
            }
            
            if (dataProvided.grid != null) {
                commands.Add(new Tuple<string, float>(c + " " + dataProvided.grid, completeness + 0.5f));
            } else {
                if (completeness != 0) commands.Add(new Tuple<string, float>(c + " (grid coord)", completeness));
            }
            
            return commands;
        }

        // Finds the closest command word within the phrase
        private static Tuple<string, int> GetClosestCommand(string phrase) {
            Tuple<string, int> closestCommandWord = new Tuple<string, int>("", phrase.Length);

            string[] words = phrase.Split(' ');
            foreach (string word in words) {
                Tuple<string, int> closestWord = GetClosestWordFromList(CommandWords, word);
                if (closestWord.Item2 < closestCommandWord.Item2) closestCommandWord = closestWord;
            }

            return closestCommandWord;
        }

        // Finds out what data is provided in the phrase
        private static DataProvided GetDataProvided(string phrase) {
            DataProvided dataProvided = new DataProvided();
            dataProvided.direction = GetDirection(phrase);
            dataProvided.destination = GetCommandListIdentifier(phrase, Destinations);
            dataProvided.grid = GetGridCoord(phrase);
            dataProvided.pingType = GetCommandListIdentifier(phrase, PingTypes);
            dataProvided.resource = GetCommandFromList(phrase, Resources);
            dataProvided.activatableObject = GetCommandListIdentifier(phrase, Activatable);
            dataProvided.lockTarget = GetCommandListIdentifier(phrase, LockTargets);
            dataProvided.stationModule = GetCommandListIdentifier(phrase, StationModules);

            return dataProvided;
        }

        // Returns the word which has the smallest levenshtein distance in the list and its distance
        private static Tuple<string, int> GetClosestWordFromList(List<string> list, string word) {
            string closestWord = "";
            int distance = word.Length;
            
            foreach (string word1 in list) {
                int d = GetLevenshteinDistance(word1, word);
                if (d < distance) {
                    distance = d;
                    closestWord = word1;
                }
            }

            return new Tuple<string, int>(closestWord, distance);
        }

        // Returns the Levenshtein distance (number of single character edits required to make the strings equal)
        private static int GetLevenshteinDistance(string word1, string word2) {
            if (word1.Length == 0) return word2.Length;
            if (word2.Length == 0) return word1.Length;

            var distance = new int[word1.Length + 1][];
            for (int index = 0; index < word1.Length + 1; index++)
            {
                distance[index] = new int[word2.Length + 1];
            }

            for (var i = 0; i <= word1.Length; i++) {
                distance[i][0] = i;
            }

            for (var j = 0; j <= word2.Length; j++) {
                distance[0][j] = j;
            }

            for (var i = 1; i <= word1.Length; i++) {
                for (var j = 1; j <= word2.Length; j++) { 
                    var cost = (word2[j - 1] == word1[i - 1]) ? 0 : 1; 
                    distance[i][j] = Min( 
                        distance[i - 1][j] + 1, 
                        distance[i][j - 1] + 1, 
                        distance[i - 1][j - 1] + cost 
                    ); 
                } 
            } 
            
            return distance[word1.Length][word2.Length]; 
        }

        private static int Min(int a, int b, int c) {
          return Math.Min(Math.Min(a, b), c);
        } 

        public static Command GetCommand(string phrase, PlayerData playerData, Transform player) {
            Command c = new Command();
            
            // Check if the phrase contains a command that requires more info
            foreach (List<string> commandList in CompoundCommands) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                        if (!c.IsValid()) c = GetCompoundCommand(phrase, commandList, command, playerData, player);
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
        
        private static Command GetCompoundCommand(string phrase, List<string> commandList, string command, PlayerData playerData, Transform player) {
            if (commandList.Equals(MovementCommands)) return GetMovementCommand(phrase, player);
            if (commandList.Equals(TurnCommands)) return GetTurnCommand(phrase, command, player);
            if (commandList.Equals(Ping)) return GetPingCommand(phrase);
            if (commandList.Equals(TransferCommands)) return GetTransferCommand(phrase, playerData);
            if (commandList.Equals(OffCommands)) return GetToggleCommand(phrase, false, command);
            if (commandList.Equals(OnCommands)) return GetToggleCommand(phrase, true, command);
            if (commandList.Equals(RepairCommands)) return GetRepairCommand(phrase);

            return new Command(); // Return an invalid command
        }
        
        private static Command GetMovementCommand(string phrase, Transform player) {
            string data = GetDirection(phrase);
            if (data != null) {
                // If moving using relative direction dont turn the camera
                if (RelativeDirections.Contains(data)) {
                    return new MovementCommand(MovementCommand.MovementType.Direction, data, false, MovementCommand.TurnType.None, player);
                }
                return new MovementCommand(MovementCommand.MovementType.Direction, data, false, MovementCommand.TurnType.Instant, player);
            }

            data = GetCommandListIdentifier(phrase, Destinations);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Destination, data, false, MovementCommand.TurnType.Instant, player);

            data = GetGridCoord(phrase);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Grid, data, false, MovementCommand.TurnType.Instant, player);

            return new Command(); // Return an invalid command
        }
        
        private static Command GetTurnCommand(string phrase, string command, Transform player) {
            string data = GetDirection(phrase);
            if (data != null) {
                if (InstantTurn.Contains(command)) {
                    return new MovementCommand(MovementCommand.MovementType.Direction, data, true, MovementCommand.TurnType.Instant, player);
                }
                
                if (SmoothTurn.Contains(command)) {
                    if (data == Strings.Left || data == Strings.Right) {
                        return new MovementCommand(MovementCommand.MovementType.Direction, data, true, MovementCommand.TurnType.Smooth, player);
                    }
                    return new MovementCommand(MovementCommand.MovementType.Direction, data, true, MovementCommand.TurnType.Instant, player);
                }
            }

            data = GetCommandListIdentifier(phrase, Destinations);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Destination, data, true, MovementCommand.TurnType.Instant, player);

            data = GetGridCoord(phrase);
            if (data != null) return new MovementCommand(MovementCommand.MovementType.Grid, data, true, MovementCommand.TurnType.Instant, player);

            return new Command(); // Return an invalid command
        }

        private static Command GetPingCommand(string phrase) {
            string pingType = GetCommandListIdentifier(phrase, PingTypes);
            string gridCoord = GetGridCoord(phrase);

            if (pingType != null && gridCoord != null) return new PingCommand(pingType, gridCoord);
            return new Command(); // Return an invalid command
        }

        private static Command GetTransferCommand(string phrase, PlayerData playerData) {
            if (GetCommandFromList(phrase, Resources) != null) return new TransferCommand(playerData.GetResources());
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
            
            // Must use only generic command
            if (activatableObject.Equals(Hyperdrive) && (GenericOnCommands.Contains(command) || OffCommands.Contains(command))) return new ToggleCommand(on, ToggleCommand.ObjectType.Hyperdrive);
            
            return new Command(); // Return an invalid command
        }

        private static Command GetRepairCommand(string phrase) {
            string module = GetCommandListIdentifier(phrase, StationModules);
            int? repairAmount = GetNumber(phrase);

            if (module != null) {
                return new RepairCommand(module, repairAmount);
            }
            
            return new Command();
        }
        
        private static string GetDirection(string phrase) {
            foreach (List<string> commandList in Directions) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) return command;
                }
            }
    
            return null;
        }

        private static int? GetNumber(string phrase) {
            Match coordMatch = Regex.Match(phrase, @"\d+");
            if (!coordMatch.Success) return null;

            int value = int.Parse(coordMatch.Value);
            return value;
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