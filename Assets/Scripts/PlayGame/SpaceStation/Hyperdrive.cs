using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public sealed class Hyperdrive : StationModule {
        
        private readonly Texture _activeTexture;

        public Hyperdrive(SpaceStation station) : base("Hyperdrive", GameConstants.HyperdriveMaxHealth, station, "SpaceStation/station/hyperdrive") {
            DamagedTexture = Resources.Load<Texture>(Textures.HyperdriveDamaged);
            FunctionalTexture = Resources.Load<Texture>(Textures.Hyperdrive);
            _activeTexture = Resources.Load<Texture>(Textures.HyperdriveActive);
            UpdateMesh();
        }

        // Activate the hyperdrive and win the game
        public void Activate() {
            if (IsFunctional()) {
                Mat.mainTexture = _activeTexture;
                Station.gameManager.GameOver(GameManager.GameOverType.Victory);
            } else {
                EventsManager.AddMessage("Hyperdrive is not functional");
            }
        }
        
    }
}