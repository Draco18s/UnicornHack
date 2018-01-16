using UnicornHack.Abilities;

namespace UnicornHack.Effects
{
    public class Shock : Effect
    {
        public Shock()
        {
        }

        public Shock(Game game) : base(game)
        {
        }

        public int Damage { get; set; }

        public override Effect Copy(Game game) => new Shock(game) {Damage = Damage};

        // TODO: Causing some mechanical and magical items to trigger
        // TODO: Removes slow
        public override void Apply(AbilityActivationContext abilityContext)
        {
            if (!abilityContext.Succeeded)
            {
                return;
            }

            (abilityContext.TargetEntity as Actor)?.ChangeCurrentHP(-1 * Damage);
            abilityContext.Add(new Shocked(abilityContext) {Damage = Damage});
        }
    }
}