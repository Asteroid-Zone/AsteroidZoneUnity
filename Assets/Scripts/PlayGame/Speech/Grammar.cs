using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayGame.Player;
using PlayGame.Player.Movement;
using PlayGame.Speech.Commands;
using Statics;
using UnityEngine;

namespace PlayGame.Speech {
    public static class Grammar {

        // Stores information about the data in the phrase and data about the players current situation
        private struct DataProvided {
            public string direction;
            public string destination;
            public string grid;
            public string pingType;
            public string resource;
            public string activatableObject;
            public string lockTarget;
            public string stationModule;
            public QuestType playerQuestType;
            public ToggleCommand.LockTargetType playerLockType;
            public bool miningLaser;
            public bool combatLaser;
            public bool nearStation;
            public Role role;
        }
        
        private const string LetterRegex = @"(?<![a-z])[a-z](?![a-z])"; // Should match only if the letter is not followed or preceded by another letter
        private const string NumberRegex = @"\d+";

        private static readonly List<string> A = new List<string>{"a"};
        private static readonly List<string> B = new List<string>{"b", "bee", "be"};
        private static readonly List<string> C = new List<string>{"c", "sea", "see"};
        private static readonly List<string> D = new List<string>{"d"};
        private static readonly List<string> E = new List<string>{"e"};
        private static readonly List<string> F = new List<string>{"f"};
        private static readonly List<string> G = new List<string>{"g"};
        private static readonly List<string> H = new List<string>{"h"};
        private static readonly List<string> I = new List<string>{"i", "eye"};
        private static readonly List<string> J = new List<string>{"j", "jay"};
        private static readonly List<string> K = new List<string>{"k", "kay"};
        private static readonly List<List<string>> Letters = new List<List<string>>{A, B, C, D, E, F, G, H, I, J, K};
        
        private static readonly List<string> One = new List<string>{"1"};
        private static readonly List<string> Two = new List<string>{"2", "too", "to"};
        private static readonly List<string> Three = new List<string>{"3", "free"};
        private static readonly List<string> Four = new List<string>{"4"};
        private static readonly List<string> Five = new List<string>{"5"};
        private static readonly List<string> Six = new List<string>{"6"};
        private static readonly List<string> Seven = new List<string>{"7"};
        private static readonly List<string> Eight = new List<string>{"8", "ate"};
        private static readonly List<string> Nine = new List<string>{"9"};
        private static readonly List<string> Ten = new List<string>{"10"};
        private static readonly List<string> Eleven = new List<string>{"11"};
        private static readonly List<List<string>> Numbers = new List<List<string>>{One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Eleven};
        
        // Lists containing synonyms for commands
        private static readonly List<string> MovementCommands = new List<string>{"move", "go", "travel"}; // Needs direction/destination/grid
        private static readonly List<string> MovementCommandsWrong = new List<string>{"leave", "news", "is"};
        private static readonly List<string> InstantTurn = new List<string>{"face", "look"};
        private static readonly List<string> InstantTurnWrong = new List<string>{"space"};
        private static readonly List<string> SmoothTurn = new List<string>{"turn", "rotate"};
        private static readonly List<string> TurnCommands = new List<string>(); // Initialised at startup, Needs direction/destination/grid
        private static readonly List<string> TurnCommandsWrong = new List<string>(); // Initialised at startup
        private static readonly List<string> SpeedCommands = new List<string>{Strings.Stop, Strings.Go};
        private static readonly List<string> TransferCommands = new List<string>{"transfer", "deposit", "unload", "drop", "shift"}; // Needs resources
        private static readonly List<string> MineCommands = new List<string> {"mine"};
        
        private static readonly List<string> GenericOnCommands = new List<string>{"activate", "engage", "turn on", "switch on", "start", "enable", "initiate"}; // Can be used to activate anything
        private static readonly List<string> LockCommands = new List<string>{"lock", "aim", "target", "focus"}; // Can only be used to activate a lock
        private static readonly List<string> LockCommandsWrong = new List<string>{"tiger", "famous"};
        private static readonly List<string> ShootCommands = new List<string>{"shoot", "fire"}; // Can only be used to activate laser gun or mining laser
        private static readonly List<string> ShootCommandsWrong = new List<string> {"cute", "shoe", "sheet"};
        //private static readonly List<string> OnCommands = new List<string>{GenericOnCommands, LockCommands, ShootCommands};
        private static readonly List<string> OnCommands = new List<string>(); // Is initialised to the above line at startup
        private static readonly List<string> OffCommands = new List<string>{"deactivate", "disengage", "turn off", "switch off", "stop", "disable"};
        private static readonly List<string> OffCommandsWrong = new List<string>{"sable"};
        private static readonly List<string> RepairCommands = new List<string>{"repair", "fix", "mend"};
        private static readonly List<string> RepairCommandsWrong = new List<string>{"meant", "men"};

        // Lists containing synonyms for objects
        private static readonly List<string> Pirate = new List<string>{Strings.Pirate, "enemy"};
        private static readonly List<string> PirateWrong = new List<string>{"anime"};
        private static readonly List<string> Asteroid = new List<string>{Strings.Asteroid, "meteor"};
        private static readonly List<string> MiningLaser = new List<string>{Strings.MiningLaser, "mining beam", "mining ray", "mining"};
        private static readonly List<string> MiningLaserWrong = new List<string>{"morning"};
        private static readonly List<string> SpaceStation = new List<string>{Strings.SpaceStation, "space station", "base"};
        private static readonly List<string> Ping = new List<string>{Strings.Ping, "pin", "mark", "flag"};
        private static readonly List<string> PingWrong = new List<string>{"pink", "pig"};
        private static readonly List<string> Resources = new List<string>{Strings.Resources, "materials", "rock", "supplies"};
        private static readonly List<string> LaserGun = new List<string> {Strings.LaserGun, "gun", "shoot", "attack", "laser beam", "ray gun", "laser"};
        private static readonly List<string> LaserGunWrong = new List<string> {"lazer", "riser", "lazarbeam"};
        
        private static readonly List<string> Hyperdrive = new List<string>{Strings.Hyperdrive, "warp drive", "hyper drive"};
        private static readonly List<string> Hull = new List<string> {Strings.Hull, "body", "frame", "structure", "armour", "exterior"};
        private static readonly List<string> HullWrong = new List<string>{"how", "hal", "armagh", "abba"};
        private static readonly List<string> ShieldGenerator = new List<string> {Strings.ShieldGenerator, "shield"};
        private static readonly List<string> Engines = new List<string> {Strings.Engines, "thrusters"};
        private static readonly List<string> SolarPanels = new List<string> {Strings.SolarPanels, "panels", "solar"};
        private static readonly List<string> SolarPanelsWrong = new List<string> {"panos"};

        private static readonly List<string> CompassDirections = new List<string>{Strings.North, Strings.East, Strings.South, Strings.West};
        private static readonly List<string> Forward = new List<string>{Strings.Forward, "ahead", "beforward", "ford", "afford"};
        private static readonly List<string> RelativeDirections = new List<string>{Strings.Back, Strings.Left, Strings.Right};                          // todo separate into lists
        private static readonly List<List<string>> Directions = new List<List<string>>{CompassDirections, RelativeDirections};
        
        private static readonly List<List<string>> Destinations = new List<List<string>>{SpaceStation, Ping, Pirate, Asteroid};
        private static readonly List<List<string>> PingTypes = new List<List<string>>{Asteroid, Pirate};
        private static readonly List<List<string>> LockTargets = new List<List<string>>{Pirate, Asteroid};
        private static readonly List<List<string>> Activatable = new List<List<string>>{MiningLaser, LockCommands, LaserGun, Hyperdrive}; // Mining laser must be before laser gun
        private static readonly List<List<string>> GenericActivatableObjects = new List<List<string>>{Hyperdrive}; // Objects that are only activated using the generic on commands and don't need any extra data
        private static readonly List<List<string>> StationModules = new List<List<string>>{Hyperdrive, Hull, ShieldGenerator, Engines, SolarPanels};
        
        private static readonly List<List<string>> SingleCommands = new List<List<string>>{SpeedCommands, ShootCommands, MineCommands};
        private static readonly List<List<string>> CompoundCommands = new List<List<string>>{MovementCommands, TurnCommands, Ping, TransferCommands, OffCommands, OnCommands, RepairCommands};

        private static readonly List<string> CommandWords;
        
        static Grammar() {
            // Add common wrong commands
            MovementCommands.AddRange(MovementCommandsWrong);
            InstantTurn.AddRange(InstantTurnWrong);
            LockCommands.AddRange(LockCommandsWrong);
            Ping.AddRange(PingWrong);
            ShootCommands.AddRange(ShootCommandsWrong);
            OffCommands.AddRange(OffCommandsWrong);
            RepairCommands.AddRange(RepairCommandsWrong);
            Pirate.AddRange(PirateWrong);
            MiningLaser.AddRange(MiningLaserWrong);
            LaserGun.AddRange(LaserGunWrong);
            Hull.AddRange(HullWrong);
            SolarPanels.AddRange(SolarPanelsWrong);
            
            TurnCommandsWrong.AddRange(InstantTurnWrong);
            
            RelativeDirections.AddRange(Forward);
            
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
        
        // Finds out what data is provided in the phrase and from PlayerData
        private static DataProvided GetDataProvided(string phrase, PlayerData playerData, MoveObject moveObject, bool miningLaser, bool combatLaser) {
            DataProvided dataProvided = new DataProvided();
            dataProvided.direction = GetDirection(phrase);
            dataProvided.destination = GetCommandListIdentifier(phrase, Destinations);
            dataProvided.grid = GetGridCoord(phrase);
            dataProvided.pingType = GetCommandListIdentifier(phrase, PingTypes);
            dataProvided.resource = GetCommandFromList(phrase, Resources);
            dataProvided.activatableObject = GetCommandListIdentifier(phrase, Activatable);
            dataProvided.lockTarget = GetCommandListIdentifier(phrase, LockTargets);
            dataProvided.stationModule = GetCommandListIdentifier(phrase, StationModules);

            dataProvided.playerQuestType = playerData.currentQuest;
            dataProvided.miningLaser = miningLaser;
            dataProvided.combatLaser = combatLaser;
            dataProvided.role = playerData.GetRole();

            if (moveObject == null) {
                dataProvided.nearStation = false;
                dataProvided.playerLockType = ToggleCommand.LockTargetType.None;
            } else {
                dataProvided.nearStation = moveObject.NearStation();
                dataProvided.playerLockType = moveObject.GetLockType();
            }

            return dataProvided;
        }

        // Returns the command which was most likely intended by the player and a measure of how confident we are that its the correct command
        public static Tuple<string, float> GetSuggestedCommandFromData(string phrase, PlayerData playerData, MoveObject moveObject, bool miningLaser, bool combatLaser) {
            // Find commands that have some of the required data in the phrase
            List<Tuple<string, float>> partiallyCompleteCommands = GetPartiallyCompleteCommands(phrase, playerData, moveObject, miningLaser, combatLaser);
            Tuple<string, float> mostCompleteCommand = new Tuple<string, float>("", 0);
            
            if (partiallyCompleteCommands.Count != 0) {
                foreach (Tuple<string, float> command in partiallyCompleteCommands) {
                    if (command.Item2 > mostCompleteCommand.Item2) mostCompleteCommand = command;
                }
            }
            
            return mostCompleteCommand;
        }

        // Returns the closest command word using levenshtein distance and how confident we are that its the correct command
        public static Tuple<string, float> GetSuggestedCommandFromDistance(string phrase) {
            Tuple<string, int> closestCommandWord = GetClosestCommand(phrase);
            
            float charactersCorrectPercentage = closestCommandWord.Item2 / (float) closestCommandWord.Item1.Length;
            float confidence = 1 - charactersCorrectPercentage;
            if (closestCommandWord.Item1 != "") return new Tuple<string, float>(closestCommandWord.Item1, confidence);
            return new Tuple<string, float>(Strings.NoCommand, 0);
        }

        private static List<Tuple<string, float>> GetPartiallyCompleteCommands(string phrase, PlayerData playerData, MoveObject moveObject, bool miningLaser, bool combatLaser) {
            DataProvided dataProvided = GetDataProvided(phrase, playerData, moveObject, miningLaser, combatLaser);
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)

            commands.AddRange(GetPartiallyCompleteMovementCommands(phrase, dataProvided, MovementCommands, MovementCommandsWrong));
            commands.AddRange(GetPartiallyCompleteMovementCommands(phrase, dataProvided, TurnCommands, TurnCommandsWrong));
            
            commands.Add(GetPartiallyCompletePingCommand(phrase, dataProvided));
            commands.Add(GetPartiallyCompleteTransferCommand(phrase, dataProvided));
            commands.AddRange(GetPartiallyCompleteOffCommand(phrase, dataProvided));
            commands.AddRange(GetPartiallyCompleteRepairCommands(phrase, dataProvided));
            
            commands.AddRange(GetPartiallyCompleteOnCommands(phrase, dataProvided));

            return commands;
        }

        private static List<Tuple<string, float>> GetPartiallyCompleteRepairCommands(string phrase, DataProvided dataProvided) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)
            if (dataProvided.role != Role.StationCommander) return commands; // Only station commander can perform these commands
            
            float completeness = 0; // Completeness of command
            float completenessAmount = 0; // Completeness of command with repair amount
            float third = 1f / 3;

            string c = "";
            
            // Add the command word they used or the default repair command if one isn't found
            foreach (string command in RepairCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    completenessAmount = third;
                    if (!RepairCommandsWrong.Contains(command)) c = command; // Use the command if its not a common mistake
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

            // Command without repair amount
            if (dataProvided.stationModule == Strings.Hull) commands.Add(new Tuple<string, float>(c, 1)); // Hull is only used for repair command so if its included assume they mean repair
            else commands.Add(new Tuple<string, float>(c, completeness)); 
            
            // Repair amount
            int? repairAmount = GetNumberRegex(phrase);
            if (repairAmount != null) {
                completenessAmount += third;
                c += " " + repairAmount;
            } else {
                c += " (repair amount)";
            }

            // Command with repair amount
            if (dataProvided.stationModule != null && repairAmount != null) commands.Add(new Tuple<string, float>(c, 1)); // If they have both number and module they must be trying to repair
            else commands.Add(new Tuple<string, float>(c, completenessAmount));

            return commands;
        }

        private static List<Tuple<string, float>> GetPartiallyCompleteLockCommands(string phrase, DataProvided dataProvided) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>();
            if (dataProvided.role == Role.StationCommander) return commands; // Only miners can perform these commands
            float completeness = 0;

            string c = "";
            
            // Add the command word they used or the default lock command if one isn't found
            foreach (string command in LockCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    if (!LockCommandsWrong.Contains(command)) c = command; // Use the command if its not a common mistake
                }
            }
            if (c == "") c = LockCommands[0];
            
            // Lock target
            if (dataProvided.lockTarget != null) {
                completeness += 0.5f;
                c += " " + dataProvided.lockTarget;
                commands.Add(new Tuple<string, float>(c, completeness));
            } else {
                commands.Add(new Tuple<string, float>(c + " (lock target)", completeness));

                float asteroidConfidence = completeness;
                float pirateConfidence = completeness;
                
                // Commands influenced by quest type
                // 0.2f so its not enough for it to be automatically done but should ask the user
                if (dataProvided.playerQuestType == QuestType.MineAsteroids) asteroidConfidence += 0.2f;
                if (dataProvided.playerQuestType == QuestType.ReturnToStationDefend) pirateConfidence += 0.2f;
                if (dataProvided.playerQuestType == QuestType.PirateWarning) pirateConfidence += 0.2f;
                
                // Less likely to try and lock onto something you're already locked onto
                if (dataProvided.playerLockType == ToggleCommand.LockTargetType.Asteroid) asteroidConfidence += 0.1f;
                if (dataProvided.playerLockType == ToggleCommand.LockTargetType.Pirate) pirateConfidence += 0.1f;
                
                commands.Add(new Tuple<string, float>(c + " " + Asteroid[0], asteroidConfidence));
                commands.Add(new Tuple<string, float>(c + " " + Pirate[0], pirateConfidence));
            }
            
            return commands;
        }
        
        private static List<Tuple<string, float>> GetPartiallyCompleteLaserCommands(string phrase, DataProvided dataProvided) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)
            if (dataProvided.role == Role.StationCommander) return commands; // Only miners can perform these commands
            float completeness = 0;

            string c = "";
            
            // Add the command word they used or the default on command if one isn't found
            foreach (string command in ShootCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    if (!ShootCommandsWrong.Contains(command)) c = command; // Use the command if its not a common mistake
                    else c = ShootCommands[0];
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
                // If they have tried to use a laser/mining laser they must be trying to turn it on/off
                // We assume they want to turn it on because nothing bad can happen by not being able to stop them but they could die if they cant shoot
                if (MiningLaser.Contains(dataProvided.activatableObject)) commands.Add(new Tuple<string, float>(c + " " + dataProvided.activatableObject, 0.9f));
                if (LaserGun.Contains(dataProvided.activatableObject)) commands.Add(new Tuple<string, float>(c + " " + dataProvided.activatableObject, 0.9f));
            } else {
                if (completeness != 0) {
                    float miningConfidence = completeness;
                    float combatConfidence = completeness;
                    
                    // Confidence adjustments for quest type
                    if (dataProvided.playerQuestType == QuestType.MineAsteroids) { // If mining quest they are more likely to turn on the mining laser
                        miningConfidence += 0.2f; // Prompt the user, not enough to perform the command
                    } else if (dataProvided.playerQuestType == QuestType.ReturnToStationDefend || dataProvided.playerQuestType == QuestType.PirateWarning) {  // If pirate quest they are more likely to turn on the combat laser
                        // Dont want them to be under attack and not be able to shoot so we are confident enough to just turn it on
                        combatConfidence += 0.4f;
                    }

                    // Confidence adjustments for lock type
                    if (dataProvided.playerLockType == ToggleCommand.LockTargetType.Asteroid) miningConfidence += 0.2f;
                    if (dataProvided.playerLockType == ToggleCommand.LockTargetType.Pirate) combatConfidence += 0.2f;
                    
                    commands.Add(new Tuple<string, float>(c + " " + MiningLaser[0], miningConfidence));
                    commands.Add(new Tuple<string, float>(c + " " + LaserGun[0], combatConfidence));
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
                        if (commandList == Hyperdrive) {
                            if (dataProvided.role == Role.StationCommander) commands.Add(new Tuple<string, float>(c + " " + dataProvided.activatableObject, Math.Max(completeness + 0.5f, 0.6f)));
                        } else commands.Add(new Tuple<string, float>(c + " " + dataProvided.activatableObject, completeness + 0.5f));
                    }
                }
            } else {
                if (completeness != 0) {
                    foreach (List<string> commandList in GenericActivatableObjects) {
                        if (commandList == Hyperdrive) {
                            // Hyperdrive is the only thing that the station commander can activate so if they used an on command this is very likely
                            if (completeness > 0 && dataProvided.role == Role.StationCommander) commands.Add(new Tuple<string, float>(c + " " + commandList[0], 0.7f));
                        } else commands.Add(new Tuple<string, float>(c + " " + commandList[0], completeness));
                    }
                }
            }
            
            return commands;
        }
        
        private static List<Tuple<string, float>> GetPartiallyCompleteOnCommands(string phrase, DataProvided dataProvided) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)

            commands.AddRange(GetPartiallyCompleteLockCommands(phrase, dataProvided));
            commands.AddRange(GetPartiallyCompleteLaserCommands(phrase, dataProvided));
            commands.AddRange(GetPartiallyCompleteGenericOnCommands(phrase, dataProvided));

            return commands;
        }
        
        // Returns an off command with the percentage of data provided
        private static List<Tuple<string, float>> GetPartiallyCompleteOffCommand(string phrase, DataProvided dataProvided) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>();
            if (dataProvided.role == Role.StationCommander) return commands; // Only miners can perform these commands
            float completeness = 0;

            string c = "";
            
            // Add the command word they used or the default off command if one isn't found
            foreach (string command in OffCommands) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    if (!OffCommandsWrong.Contains(command)) c = command; // Use the command if its not a common mistake
                }
            }
            if (c == "") c = OffCommands[0];
            
            // Activatable Object
            if (dataProvided.activatableObject != null) {
                completeness += 0.5f;
                c += " " + dataProvided.activatableObject;
                commands.Add(new Tuple<string, float>(c, completeness));
            } else {
                // More likely to turn something off if its already on
                if (dataProvided.playerLockType != ToggleCommand.LockTargetType.None) commands.Add(new Tuple<string, float>(c + " " + LockCommands[0], completeness + 0.2f));
                if (dataProvided.miningLaser) commands.Add(new Tuple<string, float>(c + " " + MiningLaser[0], completeness + 0.2f));
                if (dataProvided.combatLaser) commands.Add(new Tuple<string, float>(c + " " + LaserGun[0], completeness + 0.2f));
                
                c += " (activatable object)";
                commands.Add(new Tuple<string, float>(c, completeness));
            }

            return commands;
        }
        
        // Returns a transfer command with the percentage of data provided
        private static Tuple<string, float> GetPartiallyCompleteTransferCommand(string phrase, DataProvided dataProvided) {
            float completeness = 0;
            if (dataProvided.role == Role.StationCommander) return new Tuple<string, float>("transfer resources", completeness); // Only miners can perform these commands

            string c = "";
            
            // Add the command word they used or the default transfer command if one isn't found
            foreach (string command in TransferCommands) {
                if (phrase.Contains(command)) {
                    completeness = 1f;
                    c = command;
                }
            }
            if (c == "") c = TransferCommands[0];
            
            // Resource
            if (dataProvided.resource != null) {
                completeness = 1f;
                c += " " + dataProvided.resource;
            } else {
                c += " " + Resources[0];
            }

            if (completeness < 1f) {
                float confidence = completeness;
                if (dataProvided.playerQuestType == QuestType.ReturnToStationResources) confidence += 0.3f;
                if (dataProvided.nearStation) confidence += 0.2f;
                return new Tuple<string, float>(c, confidence);
            } else {
                // If either the transfer command or resources is in the phrase we know they are trying to transfer resources
                return new Tuple<string, float>(c, completeness);
            }
        }

        // Returns a ping command with the percentage of data provided
        private static Tuple<string, float> GetPartiallyCompletePingCommand(string phrase, DataProvided dataProvided) {
            if (dataProvided.role != Role.StationCommander) return new Tuple<string, float>("ping", 0); // Only station commanders can perform these commands
            float completeness = 0;
            float third = 1f / 3;

            string c = "";
            
            // Add the command word they used or the default ping command if one isn't found
            foreach (string command in Ping) {
                if (phrase.Contains(command)) {
                    completeness = third;
                    if (!PingWrong.Contains(command)) c = command; // Use the command if its not a common mistake
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
            
            // If command has ping type and grid coord it must be a ping command
            if (dataProvided.pingType != null && dataProvided.grid != null) return new Tuple<string, float>(c, 1);
            return new Tuple<string, float>(c, completeness);
        }

        private static List<Tuple<string, float>> GetPartiallyCompleteMovementCommands(string phrase, DataProvided dataProvided, List<string> commandList, List<string> commandListWrong) {
            List<Tuple<string, float>> commands = new List<Tuple<string, float>>(); // List of (command, dataRequiredPercentage)

            float completeness = 0;
            string c = "";
            
            // Check for command word
            foreach (string command in commandList) {
                if (phrase.Contains(command)) {
                    completeness = 0.5f;
                    if (!commandListWrong.Contains(command)) c = command; // Use the command if its not a common mistake
                }
            }
            if (c == "") c = commandList[0];

            if (dataProvided.direction != null) {
                // If forward we know its a movement command
                if (dataProvided.direction == Strings.Forward) commands.Add(new Tuple<string, float>("move forward", 1));
                else commands.Add(new Tuple<string, float>(c + " " + dataProvided.direction, completeness + 0.5f));
            } else {
                if (completeness != 0) commands.Add(new Tuple<string, float>(c + " (direction)", completeness));
            }
            
            if (dataProvided.destination != null) {
                commands.Add(new Tuple<string, float>(c + " to " + dataProvided.destination, completeness + 0.5f));
            } else {
                float asteroidConfidence = completeness;
                float pirateConfidence = completeness;
                
                // Confidence adjustments for quest type
                if (dataProvided.playerQuestType == QuestType.ReturnToStationResources || dataProvided.playerQuestType == QuestType.ReturnToStationDefend) commands.Add(new Tuple<string, float>(c + " to " + SpaceStation[0], completeness + 0.2f)); // 0.2f so it prompts the player
                if (dataProvided.playerQuestType == QuestType.MineAsteroids) asteroidConfidence += 0.2f; // 0.2f so it prompts the player
                if (dataProvided.playerQuestType == QuestType.PirateWarning) pirateConfidence += 0.2f; // 0.2f so it prompts the player
                
                // Confidence adjustments for lock type
                if (dataProvided.playerLockType == ToggleCommand.LockTargetType.Asteroid) asteroidConfidence += 0.2f;
                if (dataProvided.playerLockType == ToggleCommand.LockTargetType.Pirate) pirateConfidence += 0.2f;
                
                commands.Add(new Tuple<string, float>(c + " to " + Asteroid[0], asteroidConfidence));
                commands.Add(new Tuple<string, float>(c + " to " + Pirate[0], pirateConfidence));
                if (completeness != 0) commands.Add(new Tuple<string, float>(c + " to (destination)", completeness));
            }
            
            if (dataProvided.grid != null) {
                commands.Add(new Tuple<string, float>(c + " to " + dataProvided.grid, completeness + 0.5f));
            } else {
                if (completeness != 0) commands.Add(new Tuple<string, float>(c + " to (grid coord)", completeness));
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
                            if (commandList == MineCommands) return new ToggleCommand(true, ToggleCommand.ObjectType.MiningLaser); // Turn mining laser on
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
            int? repairAmount = GetNumberRegex(phrase);

            if (module != null) {
                return new RepairCommand(module, repairAmount);
            }
            
            return new Command();
        }
        
        private static string GetDirection(string phrase) {
            foreach (List<string> commandList in Directions) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                        if (Forward.Contains(command)) return Forward[0];
                        return command;
                    }
                }
            }
    
            return null;
        }

        private static int? GetNumberRegex(string phrase) {
            Match coordMatch = Regex.Match(phrase, NumberRegex);
            if (!coordMatch.Success) return null;

            int value = int.Parse(coordMatch.Value);
            return value;
        }

        private static string GetLetterRegex(string phrase) {
            Match coordMatch = Regex.Match(phrase, LetterRegex);
            if (!coordMatch.Success) return null;
            return coordMatch.Value;
        }

        private static string GetLetter(string phrase) {
            string letter = GetLetterRegex(phrase);
            if (letter != null) return letter;
            
            foreach (List<string> letterList in Letters) {
                foreach (string l in letterList) {
                    if (l != letterList[0] && phrase.Contains(l)) letter = letterList[0]; // Dont check for actual letters (a, b, c ..). Only check for mistakes otherwise 'Bee' would be recognised as 'e'
                }
            }

            return letter;
        }

        private static string GetNumber(string phrase) {
            int? number = GetNumberRegex(phrase);
            if (number != null) return number.ToString();

            string numberString = null;
            
            foreach (List<string> numberList in Numbers) {
                foreach (string n in numberList) {
                    if (n != numberList[0] && phrase.Contains(n)) numberString = numberList[0]; // Dont check for actual numbers (1, 2, 3 ..)
                }
            }

            return numberString;
        }
        
        public static string GetGridCoord(string phrase) {
            string letter = GetLetter(phrase);
            string number = GetNumber(phrase);

            if (letter == null || number == null) return null;
            return letter + number;
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