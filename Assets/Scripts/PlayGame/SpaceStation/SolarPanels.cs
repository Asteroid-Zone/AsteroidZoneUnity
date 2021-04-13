using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public sealed class SolarPanels : StationModule {

        public SolarPanels(SpaceStation station) : base("Solar Panels", GameConstants.SolarPanelsMaxHealth, station, "SpaceStation/station/solar_panels") {
            DamagedTexture = Resources.Load<Texture>(Textures.SolarPanelsDamaged);
            FunctionalTexture = Resources.Load<Texture>(Textures.SolarPanels);
            UpdateMesh();
        }
        
    }
}