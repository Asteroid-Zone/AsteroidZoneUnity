using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    
    /// <summary>
    /// This class controls the stations solar panels module.
    /// </summary>
    public sealed class SolarPanels : StationModule {

        /// <summary>
        /// Loads the solar panels textures.
        /// </summary>
        /// <param name="station"></param>
        public SolarPanels(SpaceStation station) : base("Solar Panels", GameConstants.SolarPanelsMaxHealth, station, "SpaceStation/station/solar_panels") {
            DamagedTexture = Resources.Load<Texture>(Textures.SolarPanelsDamaged);
            FunctionalTexture = Resources.Load<Texture>(Textures.SolarPanels);
            UpdateMesh();
        }
        
    }
}