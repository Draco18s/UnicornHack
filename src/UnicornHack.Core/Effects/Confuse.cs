namespace UnicornHack.Effects
{
    public class Confuse : Effect
    {
        public Confuse()
        {
        }

        public Confuse(Game game)
            : base(game)
        {
        }

        public int Duration { get; set; }

        public override Effect Instantiate(Game game)
            => new Confuse(game) {Duration = Duration};
    }
}