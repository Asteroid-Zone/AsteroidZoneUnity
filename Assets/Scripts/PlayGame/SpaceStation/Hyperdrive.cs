using PlayGame.UI;
using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    
    /// <summary>
    /// This class controls the stations hyperdrive module.
    /// </summary>
    public sealed class Hyperdrive : StationModule {
        
        private readonly Texture _activeTexture;

        /// <summary>
        /// Loads the hyperdrive textures.
        /// </summary>
        /// <param name="station"></param>
        public Hyperdrive(SpaceStation station) : base("Hyperdrive", GameConstants.HyperdriveMaxHealth, station, "SpaceStation/station/hyperdrive") {
            DamagedTexture = Resources.Load<Texture>(Textures.HyperdriveDamaged);
            FunctionalTexture = Resources.Load<Texture>(Textures.Hyperdrive);
            _activeTexture = Resources.Load<Texture>(Textures.HyperdriveActive);
            UpdateMesh();
        }

        /// <summary>
        /// Activates the hyperdrive and wins the game.
        /// </summary>
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