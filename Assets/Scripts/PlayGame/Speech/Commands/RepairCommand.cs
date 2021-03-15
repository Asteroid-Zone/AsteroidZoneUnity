using System;
using Statics;

namespace PlayGame.Speech.Commands {
    public class RepairCommand : Command {

        public enum StationModule {
            Hyperdrive,
            Hull,
            ShieldGenerator
        }

        public readonly StationModule stationModule;
        public readonly int? repairAmount;

        public RepairCommand(string stationModule, int? repairAmount) : base(CommandType.Repair, true) {
            this.stationModule = GetStationModuleFromString(stationModule);
            this.repairAmount = repairAmount;
        }

        private static StationModule GetStationModuleFromString(string module) {
            switch (module) {
                case Strings.Hyperdrive:
                    return StationModule.Hyperdrive;
                case Strings.Hull:
                    return StationModule.Hull;
                case Strings.ShieldGenerator:
                    return StationModule.ShieldGenerator;
                default:
                    throw new ArgumentException("Invalid Station Module");
            }
        }
        
    }
}