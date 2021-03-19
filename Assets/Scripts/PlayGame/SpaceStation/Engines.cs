using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class Engines : StationModule {

        private const int MaxHealth = 100;
        
        private readonly Material _materialEngine1;
        private Texture _damagedTextureEngine1;
        private Texture _functionalTextureEngine1;

        private readonly Material _materialEngine2;
        private Texture _damagedTextureEngine2;
        private Texture _functionalTextureEngine2;
        public Engines(SpaceStation station) : base("Engines", MaxHealth, station) {
            _materialEngine1 = station.transform.Find("station/engines/engine1").gameObject.GetComponent<Renderer>().material;
            _damagedTextureEngine1 = Resources.Load<Texture>(Textures.Engine1Damaged);
            _functionalTextureEngine1 = Resources.Load<Texture>(Textures.Engine1);
            
            _materialEngine2 = station.transform.Find("station/engines/engine2").gameObject.GetComponent<Renderer>().material;
            _damagedTextureEngine2 = Resources.Load<Texture>(Textures.Engine2Damaged);
            _functionalTextureEngine2 = Resources.Load<Texture>(Textures.Engine2);

            UpdateMesh();
        }
        
        protected override void UpdateMesh() {
            _materialEngine1.mainTexture = IsFunctional() ? _functionalTextureEngine1 : _damagedTextureEngine1;
            _materialEngine2.mainTexture = IsFunctional() ? _damagedTextureEngine2 : _functionalTextureEngine2;
        }
        
    }
}