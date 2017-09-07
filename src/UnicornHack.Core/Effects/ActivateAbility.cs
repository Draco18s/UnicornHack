namespace UnicornHack.Effects
{
    public class ActivateAbility : Effect
    {
        public ActivateAbility()
        {
        }

        public ActivateAbility(Game game) : base(game)
        {
        }

        public Ability Ability { get; set; }

        public override Effect Copy(Game game)
            => new ActivateAbility(game) {Ability = Ability, Duration = Duration};

        public override void Apply(AbilityActivationContext abilityContext)
        {
            Ability.Activate(new AbilityActivationContext
            {
                Activator = abilityContext.Activator,
                Target = abilityContext.Target,
                IsAttack = abilityContext.IsAttack
            });
        }

        public override void Delete()
        {
            base.Delete();

            Ability.RemoveReference();
            Ability = null;
        }
    }
}