using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class SolarPanels : StationModule {

        private const int MaxHealth = 100;

        public SolarPanels(SpaceStation station) : base("Solar Panels", MaxHealth, station, "station/solar_panels") {
            _damagedTexture = Resources.Load<Texture>(Textures.SolarPanelsDamaged);
            _functionalTexture = Resources.Load<Texture>(Textures.SolarPanels);
            UpdateMesh();
        }
        
    }
}