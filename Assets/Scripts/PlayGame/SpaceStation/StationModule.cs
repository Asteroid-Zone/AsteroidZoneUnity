using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayGame.SpaceStation {
    public class StationModule {
        
        protected readonly SpaceStation Station;

        protected Material Mat;
        protected Texture DamagedTexture;
        protected Texture FunctionalTexture;
        
        public readonly string Name;
        public readonly int MaxHealth;
        public int ModuleHealth;

        protected StationModule(string name, int maxHealth, SpaceStation station, string path) {
            Name = name;
            MaxHealth = maxHealth;
            Station = station;
            ModuleHealth = Random.Range(0, maxHealth / 2); // Set initial health to be less than half
            Debug.Log(path);
            Mat = station.transform.Find(path).gameObject.GetComponent<Renderer>().material;
            DamagedTexture = Texture2D.redTexture;
            FunctionalTexture = Texture2D.whiteTexture;
            UpdateMesh();
        }
        
        protected StationModule(string name, int maxHealth, SpaceStation station) {
            Name = name;
            MaxHealth = maxHealth;
            Station = station;
            ModuleHealth = Random.Range(0, maxHealth / 2); // Set initial health to be less than half
        }

        protected virtual void UpdateMesh() {
            Mat.mainTexture = IsFunctional() ? FunctionalTexture : DamagedTexture;
        }

        public void Repair(int resources) {
            int repairAmount = GetRepairAmount(resources);
            
            ModuleHealth += repairAmount;
            Station.resources -= repairAmount;
            if (IsFunctional()) UpdateMesh();
        }

        // Returns the repair amount, minimum of station resources, remaining health to repair and the amount chosen by the player
        private int GetRepairAmount(int resources) {
            return Math.Min(Math.Min(MaxHealth - ModuleHealth, Station.resources), resources);
        }

        public bool IsFunctional() {
            return ModuleHealth >= MaxHealth;
        }

        public virtual void TakeDamage(int damage) {
            ModuleHealth -= damage;
            if (ModuleHealth < 0) ModuleHealth = 0;
            if (!IsFunctional()) UpdateMesh();
        }

        public override string ToString() {
            return Name + ": " + ModuleHealth + "/" + MaxHealth;
        }
    }
}