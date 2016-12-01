new CreatureVariant
{
    Name = "queen bee",
    Species = Species.Bee,
    SpeciesClass = SpeciesClass.Vermin,
    InitialLevel = 9,
    ArmorClass = -4,
    MovementRate = 24,
    Weight = 5,
    Size = Size.Tiny,
    Nutrition = 5,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Sting,
            Timeout = 1,
            Effects = new AbilityEffect[] { new VenomDamage { Damage = 4 } }
        }
,
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new AbilityEffect[] { new PoisonDamage { Damage = 8 } } }
    }
,
    SimpleProperties = new HashSet<string> { "Flight", "FlightControl", "AnimalBody", "Handlessness", "Femaleness" },
    ValuedProperties = new Dictionary<string, Object> { { "PoisonResistance", 3 } },
    GenerationFlags = GenerationFlags.Entourage,
    GenerationFrequency = Frequency.Rarely,
    Noise = ActorNoiseType.Buzz
}
