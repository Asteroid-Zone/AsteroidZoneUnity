namespace PlayGame.SpaceStation {
    public class StationHull : StationModule {

        private const int MaxHealth = 100;
        
        public StationHull(SpaceStation station) : base("Station Hull", MaxHealth, station) {
            moduleHealth += MaxHealth / 2; // Increase the minimum starting health to half
        }

        public override void TakeDamage(int damage) {
            base.TakeDamage(damage);
            if (moduleHealth <= 0) spaceStation.GameOver(false); // Space station destroyed
        }
    }
}