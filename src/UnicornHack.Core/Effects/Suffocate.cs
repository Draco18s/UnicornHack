namespace UnicornHack.Effects
{
    public class Suffocate : Effect
    {
        public Suffocate()
        {
        }

        public Suffocate(Game game) : base(game)
        {
        }

        public override Effect Copy(Game game) => new Suffocate(game);

        public override void Apply(AbilityActivationContext abilityContext)
        {
            if (!abilityContext.Succeeded)
            {
                return;
            }

            abilityContext.AppliedEffects.Add(new Suffocated(abilityContext) {Duration = Duration});
        }
    }
}