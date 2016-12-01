new CreatureVariant
{
    Name = "cobra",
    Species = Species.Snake,
    SpeciesClass = SpeciesClass.Reptile,
    InitialLevel = 7,
    ArmorClass = 2,
    MovementRate = 18,
    Weight = 250,
    Size = Size.Medium,
    Nutrition = 100,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new VenomDamage { Damage = 5 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Spit,
            Timeout = 1,
            Effects = new AbilityEffect[] { new Blind { Duration = 5 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Swimming", "Concealment", "Infravision", "SerpentlikeBody", "Limblessness", "Oviparity", "Carnivorism", "NoInventory" },
    ValuedProperties = new Dictionary<string, Object> { { "PoisonResistance", 3 }, { "VenomResistance", 3 } },
    GenerationFrequency = Frequency.Uncommonly,
    Noise = ActorNoiseType.Hiss
}