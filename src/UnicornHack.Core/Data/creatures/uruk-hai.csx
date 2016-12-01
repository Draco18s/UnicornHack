new CreatureVariant
{
    Name = "uruk-hai",
    Species = Species.Orc,
    InitialLevel = 4,
    ArmorClass = 10,
    MovementRate = 9,
    Weight = 1300,
    Size = Size.Medium,
    Nutrition = 300,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnMeleeAttack,
            Action = AbilityAction.Modifier,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 4 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 1 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Infravision", "Infravisibility", "Humanoidness", "Omnivorism" },
    GenerationFlags = GenerationFlags.LargeGroup,
    GenerationFrequency = Frequency.Commonly,
    Behavior = MonsterBehavior.GoldCollector | MonsterBehavior.WeaponCollector,
    Alignment = -5,
    Noise = ActorNoiseType.Grunt
}
