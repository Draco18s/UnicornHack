namespace UnicornHack.Effects
{
    public class DrainEnergy : Effect
    {
        public DrainEnergy()
        {
        }

        public DrainEnergy(Game game)
            : base(game)
        {
        }

        public int Amount { get; set; }

        public override Effect Instantiate(Game game)
            => new DrainEnergy(game) {Amount = Amount};
    }
}