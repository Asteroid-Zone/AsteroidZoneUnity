namespace PlayGame.SpaceStation {
    public class StationHull : StationModule {

        private const int MaxHealth = 100;

        private readonly SpaceStation _spaceStation;
        
        public StationHull(SpaceStation station) : base("Hull", MaxHealth) {
            _spaceStation = station;
            moduleHealth += MaxHealth / 2; // Increase the minimum starting health to half
        }

        public override void TakeDamage(int damage) {
            base.TakeDamage(damage);
            if (moduleHealth <= 0) _spaceStation.GameOver(false); // Space station destroyed
        }
    }
}