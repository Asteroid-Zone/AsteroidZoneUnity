using System;
using Statics;

namespace PlayGame.Speech.Commands {
    
    /// <summary>
    /// This class represents a repair command.
    /// </summary>
    public class RepairCommand : Command {

        /// <summary>
        /// Type of station module.
        /// </summary>
        public enum StationModule {
            Hyperdrive,
            Hull,
            ShieldGenerator,
            Engines,
            SolarPanels
        }

        public readonly StationModule stationModule;
        public readonly int? repairAmount;

        /// <summary>
        /// Constructor for a repair command.
        /// <remarks>Repair commands are commander only.</remarks>
        /// </summary>
        /// <param name="stationModule">A string describing the station module to repair.</param>
        /// <param name="repairAmount">The amount of resources to repair the module with.</param>
        public RepairCommand(string stationModule, int? repairAmount) : base(CommandType.Repair, true, false) {
            this.stationModule = GetStationModuleFromString(stationModule);
            this.repairAmount = repairAmount;
        }

        /// <summary>
        /// Converts a module string to a StationModule.
        /// </summary>
        /// <param name="module">A string describing the station module.</param>
        /// <exception cref="ArgumentException">Thrown if the module string is not a recognised module.</exception>
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