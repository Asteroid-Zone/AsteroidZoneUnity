using Statics;
using UnityEngine;

namespace PlayGame.SpaceStation {
    public class StationHull : StationModule {

        private const int MaxHealth = 1000;
        
        private readonly Material _materialUpperRing;
        private Texture _damagedTextureUpperRing;
        private Texture _functionalTextureUpperRing;
        
        private readonly Material _materialLowerRing;
        private Texture _damagedTextureLowerRing;
        private Texture _functionalTextureLowerRing;
        
        public StationHull(SpaceStation station) : base("Station Hull", MaxHealth, station) {
            _material = station.transform.Find("station/hull/centre_hull").gameObject.GetComponent<Renderer>().material;
            _damagedTexture = Resources.Load<Texture>(Textures.CentreHullDamaged);
            _functionalTexture = Resources.Load<Texture>(Textures.CentreHull);
            
            _materialUpperRing = station.transform.Find("station/hull/upper_ring").gameObject.GetComponent<Renderer>().material;
            _damagedTextureUpperRing = Resources.Load<Texture>(Textures.UpperRingDamaged);
            _functionalTextureUpperRing = Resources.Load<Texture>(Textures.UpperRing);

            _materialLowerRing = station.transform.Find("station/hull/lower_ring").gameObject.GetComponent<Renderer>().material;
            _damagedTextureLowerRing = Resources.Load<Texture>(Textures.LowerRingDamaged);
            _functionalTextureLowerRing = Resources.Load<Texture>(Textures.LowerRing);
            
            UpdateMesh();
            moduleHealth += MaxHealth / 2; // Increase the minimum starting health to half
        }

        protected override void UpdateMesh() {
            _material.mainTexture = IsFunctional() ? _functionalTexture : _damagedTexture;
            _materialUpperRing.mainTexture = IsFunctional() ? _functionalTextureUpperRing : _damagedTextureUpperRing;
            _materialLowerRing.mainTexture = IsFunctional() ? _functionalTextureLowerRing : _damagedTextureLowerRing;
        }
        
        public override void TakeDamage(int damage) {
            base.TakeDamage(damage);
            if (moduleHealth <= 0) spaceStation.GameOver(false); // Space station destroyed
        }
    }
}