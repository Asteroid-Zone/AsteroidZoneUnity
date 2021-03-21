using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class Hyperdrive : StationModule {

        private const int MaxHealth = 100;

        public Hyperdrive(SpaceStation station) : base("Hyperdrive", MaxHealth, station, "SpaceStation/station/hyperdrive") {
            _damagedTexture = Resources.Load<Texture>(Textures.HyperdriveDamaged);
            _functionalTexture = Resources.Load<Texture>(Textures.Hyperdrive);
            UpdateMesh();
        }

        // Activate the hyperdrive and win the game
        public void Activate() {
            if (IsFunctional()) spaceStation.GameOver(true);
            else EventsManager.AddMessage("Hyperdrive is not functional");
        }
        
    }
}