using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    
    /// <summary>
    /// This class controls the stations engines module.
    /// </summary>
    public sealed class Engines : StationModule {
        
        private readonly Material _materialEngine1;
        private readonly Texture _damagedTextureEngine1;
        private readonly Texture _functionalTextureEngine1;
        private readonly Texture _activeTextureEngine1;

        private readonly Material _materialEngine2;
        private readonly Texture _damagedTextureEngine2;
        private readonly Texture _functionalTextureEngine2;
        private readonly Texture _activeTextureEngine2;
        
        /// <summary>
        /// Loads the engines textures.
        /// </summary>
        /// <param name="station"></param>
        public Engines(SpaceStation station) : base("Engines", GameConstants.EnginesMaxHealth, station) {
            _materialEngine1 = station.transform.Find("SpaceStation/station/engines/engine1").gameObject.GetComponent<Renderer>().material;
            _damagedTextureEngine1 = Resources.Load<Texture>(Textures.Engine1Damaged);
            _functionalTextureEngine1 = Resources.Load<Texture>(Textures.Engine1);
            _activeTextureEngine1 = Resources.Load<Texture>(Textures.Engine1Active);
            
            _materialEngine2 = station.transform.Find("SpaceStation/station/engines/engine2").gameObject.GetComponent<Renderer>().material;
            _damagedTextureEngine2 = Resources.Load<Texture>(Textures.Engine2Damaged);
            _functionalTextureEngine2 = Resources.Load<Texture>(Textures.Engine2);
            _activeTextureEngine2 = Resources.Load<Texture>(Textures.Engine2Active);

            UpdateMesh();
        }
        
        /// <summary>
        /// Updates the engines mesh with the correct texture depending on if they are functional or not.
        /// </summary>
        protected override void UpdateMesh() {
            _materialEngine1.mainTexture = IsFunctional() ? _functionalTextureEngine1 : _damagedTextureEngine1;
            _materialEngine2.mainTexture = IsFunctional() ? _functionalTextureEngine2 : _damagedTextureEngine2;
        }

        /// <summary>
        /// Updates the engines mesh with the correct texture depending on if they are active or not.
        /// <remarks>Method can only be called if the engines are functional.</remarks>
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active) {
            if (!IsFunctional()) return;
            
            _materialEngine1.mainTexture = active ? _activeTextureEngine1 : _functionalTextureEngine1;
            _materialEngine2.mainTexture = active ? _activeTextureEngine2 : _functionalTextureEngine2;
        }

    }
}