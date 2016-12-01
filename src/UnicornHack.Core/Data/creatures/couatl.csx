new CreatureVariant
{
    Name = "couatl",
    Species = Species.WingedSnake,
    SpeciesClass = SpeciesClass.Reptile | SpeciesClass.Celestial,
    CorpseVariantName = "",
    InitialLevel = 8,
    ArmorClass = 5,
    MagicResistance = 30,
    MovementRate = 10,
    Weight = 900,
    Size = Size.Large,
    Nutrition = 400,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 7 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Hug,
            Timeout = 1,
            Effects = new AbilityEffect[] { new Bind { Duration = 7 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Flight", "FlightControl", "Infravision", "SerpentlikeBody", "SingularInventory" },
    ValuedProperties = new Dictionary<string, Object> { { "PoisonResistance", 3 }, { "VenomResistance", 3 } },
    GenerationFlags = GenerationFlags.SmallGroup | GenerationFlags.NoHell,
    GenerationFrequency = Frequency.Sometimes,
    Behavior = MonsterBehavior.AlignmentAware | MonsterBehavior.Stalking,
    Alignment = 7,
    Noise = ActorNoiseType.Hiss
}
