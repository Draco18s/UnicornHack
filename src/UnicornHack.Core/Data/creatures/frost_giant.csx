new CreatureVariant
{
    InitialLevel = 10,
    ArmorClass = 3,
    MagicResistance = 10,
    GenerationFlags = GenerationFlags.SmallGroup | GenerationFlags.NoHell,
    GenerationFrequency = Frequency.Uncommonly,
    Behavior = MonsterBehavior.GemCollector | MonsterBehavior.WeaponCollector,
    Alignment = -3,
    Noise = ActorNoiseType.Boast,
    Name = "frost giant",
    Species = Species.Giant,
    MovementRate = 12,
    Size = Size.Huge,
    Weight = 2250,
    Nutrition = 750,
    SimpleProperties = new HashSet<string> { "Infravision", "Infravisibility", "Humanoidness", "Omnivorism" },
    ValuedProperties = new Dictionary<string, Object> { { "ColdResistance", 3 } },
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnMeleeAttack,
            Action = AbilityAction.Modifier,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 11 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.Targetted,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 1 } }
        }
    }
}
