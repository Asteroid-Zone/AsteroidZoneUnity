using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class Engines : StationModule {
        
        private readonly Material _materialEngine1;
        private Texture _damagedTextureEngine1;
        private Texture _functionalTextureEngine1;
        private Texture _activeTextureEngine1;

        private readonly Material _materialEngine2;
        private Texture _damagedTextureEngine2;
        private Texture _functionalTextureEngine2;
        private Texture _activeTextureEngine2;
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
        
        protected override void UpdateMesh() {
            _materialEngine1.mainTexture = IsFunctional() ? _functionalTextureEngine1 : _damagedTextureEngine1;
            _materialEngine2.mainTexture = IsFunctional() ? _functionalTextureEngine2 : _damagedTextureEngine2;
        }

        public void SetActive(bool active) {
            if (!IsFunctional()) return;
            
            _materialEngine1.mainTexture = active ? _activeTextureEngine1 : _functionalTextureEngine1;
            _materialEngine2.mainTexture = active ? _activeTextureEngine2 : _functionalTextureEngine2;
        }

    }
}