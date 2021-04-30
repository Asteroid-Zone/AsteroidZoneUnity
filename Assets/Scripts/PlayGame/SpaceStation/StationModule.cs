using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayGame.SpaceStation {
    
    /// <summary>
    /// This class is a base class for all station modules.
    /// </summary>
    public class StationModule {
        
        protected readonly SpaceStation Station;

        protected Material Mat;
        protected Texture DamagedTexture;
        protected Texture FunctionalTexture;
        
        public readonly string Name;
        public readonly int MaxHealth;
        public int ModuleHealth;

        /// <summary>
        /// Sets the modules values and fetches the modules renderer.
        /// </summary>
        /// <param name="name">The name of the module to be displayed to the player.</param>
        /// <param name="maxHealth"></param>
        /// <param name="station"></param>
        /// <param name="path">The path of the modules mesh within the station model.</param>
        protected StationModule(string name, int maxHealth, SpaceStation station, string path) {
            Name = name;
            MaxHealth = maxHealth;
            Station = station;
            ModuleHealth = Random.Range(0, maxHealth / 2); // Set initial health to be less than half
            Mat = station.transform.Find(path).gameObject.GetComponent<Renderer>().material;
            DamagedTexture = Texture2D.redTexture;
            FunctionalTexture = Texture2D.whiteTexture;
            UpdateMesh();
        }
        
        /// <summary>
        /// Sets the modules values.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxHealth"></param>
        /// <param name="station"></param>
        protected StationModule(string name, int maxHealth, SpaceStation station) {
            Name = name;
            MaxHealth = maxHealth;
            Station = station;
            ModuleHealth = Random.Range(0, maxHealth / 2); // Set initial health to be less than half
        }

        /// <summary>
        /// Updates the modules mesh with the correct texture depending on if it is functional or not.
        /// </summary>
        protected virtual void UpdateMesh() {
            Mat.mainTexture = IsFunctional() ? FunctionalTexture : DamagedTexture;
        }

        /// <summary>
        /// Repairs the module and removes the resources from the station.
        /// </summary>
        /// <param name="resources">The amount of resources the player wants to use.</param>
        public void Repair(int resources) {
            int repairAmount = GetRepairAmount(resources);
            
            ModuleHealth += repairAmount;
            Station.resources -= repairAmount;
            if (IsFunctional()) UpdateMesh();
        }

        /// <summary>
        /// <para>Returns the amount to repair the module by.</para>
        /// Returns the mininmum of the stations resources, the remaining health to repair of the module and the amount chosen by the player.
        /// </summary>
        /// <param name="resources">The amount of resources the player wants to use.</param>
        private int GetRepairAmount(int resources) {
            return Math.Min(Math.Min(MaxHealth - ModuleHealth, Station.resources), resources);
        }

        /// <summary>
        /// Returns true if the module is fully repaired.
        /// <para>Otherwise returns false.</para>
        /// </summary>
        public bool IsFunctional() {
            return ModuleHealth >= MaxHealth;
        }

        /// <summary>
        /// Reduces the modules health by the given amount.
        /// </summary>
        /// <param name="damage"></param>
        public virtual void TakeDamage(int damage) {
            ModuleHealth -= damage;
            if (ModuleHealth < 0) ModuleHealth = 0;
            if (!IsFunctional()) UpdateMesh();
        }

        /// <summary>
        /// Returns a string containing the modules name, health and max health.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Name + ": " + ModuleHealth + "/" + MaxHealth;
        }
    }
}