new Creature
{
    Name = "water nymph",
    Species = Species.Nymph,
    SpeciesClass = SpeciesClass.Fey,
    ArmorClass = 9,
    MagicResistance = 20,
    MovementRate = 12,
    Weight = 600,
    Size = Size.Medium,
    Nutrition = 300,
    Abilities = new HashSet<Ability>
    {
        new Ability { Activation = AbilityActivation.OnTarget, Action = AbilityAction.Touch, Timeout = 1, Effects = new HashSet<Effect> { new Seduce { } } },
        new Ability { Activation = AbilityActivation.OnTarget, Action = AbilityAction.Touch, Timeout = 1, Effects = new HashSet<Effect> { new StealItem { } } }
    }
,
    SimpleProperties = new HashSet<string> { "Teleportation", "Swimming", "Humanoidness", "Infravisibility", "Femaleness" },
    InitialLevel = 3,
    GenerationFrequency = Frequency.Sometimes,
    Behavior = MonsterBehavior.WeaponCollector,
    Noise = ActorNoiseType.Seduction
}
