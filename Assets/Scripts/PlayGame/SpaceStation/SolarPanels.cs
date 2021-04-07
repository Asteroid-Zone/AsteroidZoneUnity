using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class SolarPanels : StationModule {

        public SolarPanels(SpaceStation station) : base("Solar Panels", GameConstants.SolarPanelsMaxHealth, station, "SpaceStation/station/solar_panels") {
            _damagedTexture = Resources.Load<Texture>(Textures.SolarPanelsDamaged);
            _functionalTexture = Resources.Load<Texture>(Textures.SolarPanels);
            UpdateMesh();
        }
        
    }
}