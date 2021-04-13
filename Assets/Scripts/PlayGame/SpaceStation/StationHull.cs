using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public sealed class StationHull : StationModule {

        private readonly Material _materialUpperRing;
        private readonly Texture _damagedTextureUpperRing;
        private readonly Texture _functionalTextureUpperRing;
        
        private readonly Material _materialLowerRing;
        private readonly Texture _damagedTextureLowerRing;
        private readonly Texture _functionalTextureLowerRing;
        
        public StationHull(SpaceStation station) : base("Station Hull", GameConstants.StationHullMaxHealth, station) {
            Mat = station.transform.Find("SpaceStation/station/hull/centre_hull").gameObject.GetComponent<Renderer>().material;
            DamagedTexture = Resources.Load<Texture>(Textures.CentreHullDamaged);
            FunctionalTexture = Resources.Load<Texture>(Textures.CentreHull);
            
            _materialUpperRing = station.transform.Find("SpaceStation/station/hull/upper_ring").gameObject.GetComponent<Renderer>().material;
            _damagedTextureUpperRing = Resources.Load<Texture>(Textures.UpperRingDamaged);
            _functionalTextureUpperRing = Resources.Load<Texture>(Textures.UpperRing);

            _materialLowerRing = station.transform.Find("SpaceStation/station/hull/lower_ring").gameObject.GetComponent<Renderer>().material;
            _damagedTextureLowerRing = Resources.Load<Texture>(Textures.LowerRingDamaged);
            _functionalTextureLowerRing = Resources.Load<Texture>(Textures.LowerRing);
            
            UpdateMesh();
            ModuleHealth += GameConstants.StationHullMaxHealth / 2; // Increase the minimum starting health to half
        }

        protected override void UpdateMesh() {
            Mat.mainTexture = IsFunctional() ? FunctionalTexture : DamagedTexture;
            _materialUpperRing.mainTexture = IsFunctional() ? _functionalTextureUpperRing : _damagedTextureUpperRing;
            _materialLowerRing.mainTexture = IsFunctional() ? _functionalTextureLowerRing : _damagedTextureLowerRing;
        }
        
        public override void TakeDamage(int damage) {
            base.TakeDamage(damage);
            if (ModuleHealth <= 0) Station.GameOver(false); // Space station destroyed
        }
    }
}