namespace UnicornHack.Effects
{
    public class Seduce : Effect
    {
        public Seduce()
        {
        }

        public Seduce(Game game)
            : base(game)
        {
        }

        public override Effect Instantiate(Game game)
            => new Seduce(game);
    }
}