using System;
using Statics;

namespace PlayGame.Speech.Commands {
    public class RepairCommand : Command {

        public enum StationModule {
            Hyperdrive,
            Hull,
            ShieldGenerator,
            Engines,
            SolarPanels
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
                case Strings.Engines:
                    return StationModule.Engines;
                case Strings.SolarPanels:
                    return StationModule.SolarPanels;
                default:
                    throw new ArgumentException("Invalid Station Module");
            }
        }
        
    }
}