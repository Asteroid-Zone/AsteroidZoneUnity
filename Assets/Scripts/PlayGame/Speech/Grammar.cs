using System.Collections.Generic;
using System.Linq;

namespace PlayGame.Speech {
    public class Grammar {
        
        private static string _gridCoordRegex = @"[a-z]( )?(\d+)";
        
        private static readonly List<string> CompassDirections = new List<string>{"north", "east", "south", "west"};
        private static readonly List<string> MovementCommands = new List<string>{"move", "go"};
        private static readonly List<string> TurnCommands = new List<string>{"face", "turn"};

        private static readonly List<string> PingTypes = new List<string>{"none", "asteroid", "pirate"};
        private static readonly List<string> PingCommands = new List<string>{"ping", "mark"};
        
        private static readonly List<string> Destinations = new List<string>{"space station", "station"};
        
        private static readonly List<string> SpeedCommands = new List<string>{"stop", "go"};
        
        private static readonly List<string> TransferCommands = new List<string>{"transfer", "deposit"};
        private static readonly List<string> Transferable = new List<string>{"resources", "materials"};

        private static readonly List<string> MiningLaser = new List<string>{"mining laser", "laser", "mining beam"};
        
        private static readonly List<string> LockOnCommands = new List<string>{"lock", "aim"};
        private static readonly List<string> LockTargets = new List<string>{"pirate", "asteroid"};
        
        private static readonly List<string> OnCommands = new List<string>{"activate", "engage", "turn on"};
        private static readonly List<string> OffCommands = new List<string>{"deactivate", "disengage", "turn off"};
        private static readonly List<List<string>> Activatable = new List<List<string>>{MiningLaser, LockOnCommands};
        
        private static readonly List<string> ShootCommands = new List<string>{"shoot", "fire"};

        private static readonly List<List<string>> SingleCommands = new List<List<string>>{SpeedCommands, ShootCommands};
        private static readonly List<List<string>> CompoundCommands = new List<List<string>>{MovementCommands, TurnCommands, PingCommands, TransferCommands, OnCommands, OffCommands};

        // Checks if a phrase is valid
        private static bool IsValidPhrase(string phrase) {
            // Check if the phrase contains a single word command
            foreach (List<string> commandList in SingleCommands) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) return true;
                }
            }
            
            // Check if the phrase contains a command that requires more info
            foreach (List<string> commandList in CompoundCommands) {
                foreach (string command in commandList) {
                    if (phrase.Contains(command)) {
                        if (HasValidSubject(phrase, commandList)) return true;
                    }
                }
            }

            return false;
        }

        // Calls the correct method to determine if the command has the required information
        private static bool HasValidSubject(string phrase, List<string> commandList) {
            if (commandList.Equals(MovementCommands)) return HasDirection(phrase);
            if (commandList.Equals(TurnCommands)) return HasDirection(phrase);
            if (commandList.Equals(PingCommands)) return HasPingType(phrase);
            if (commandList.Equals(TransferCommands)) return HasTransferableObject(phrase);
            if (commandList.Equals(OnCommands)) return HasActivatableObject(phrase);
            if (commandList.Equals(OffCommands)) return HasActivatableObject(phrase);

            return false;
        }

        private static bool HasDirection(string phrase) {
            throw new System.NotImplementedException();
        }
        
        private static bool HasPingType(string phrase) {
            throw new System.NotImplementedException();
        }
        
        private static bool HasTransferableObject(string phrase) {
            throw new System.NotImplementedException();
        }
        
        private static bool HasActivatableObject(string phrase) {
            throw new System.NotImplementedException();
        }

    }
}