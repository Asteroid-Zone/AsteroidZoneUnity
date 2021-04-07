using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class Hyperdrive : StationModule {
        
        private Texture _activeTexture;

        public Hyperdrive(SpaceStation station) : base("Hyperdrive", GameConstants.HyperdriveMaxHealth, station, "SpaceStation/station/hyperdrive") {
            _damagedTexture = Resources.Load<Texture>(Textures.HyperdriveDamaged);
            _functionalTexture = Resources.Load<Texture>(Textures.Hyperdrive);
            _activeTexture = Resources.Load<Texture>(Textures.HyperdriveActive);
            UpdateMesh();
        }

        // Activate the hyperdrive and win the game
        public void Activate() {
            if (IsFunctional()) {
                _material.mainTexture = _activeTexture;
                spaceStation.GameOver(true);
            } else {
                EventsManager.AddMessage("Hyperdrive is not functional");
            }
        }
        
    }
}