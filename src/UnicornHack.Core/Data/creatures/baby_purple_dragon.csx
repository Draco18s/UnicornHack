new CreatureVariant
{
    Name = "baby purple dragon",
    Species = Species.Dragon,
    SpeciesClass = SpeciesClass.Reptile,
    NextStageName = "purple dragon",
    InitialLevel = 12,
    ArmorClass = 2,
    MagicResistance = 10,
    MovementRate = 9,
    Weight = 1500,
    Size = Size.Large,
    Nutrition = 500,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 7 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Flight", "FlightControl", "Infravision", "AnimalBody", "Handlessness", "Carnivorism", "SingularInventory" },
    ValuedProperties = new Dictionary<string, Object> { { "VenomResistance", 3 }, { "PoisonResistance", 3 }, { "ThickHide", 3 } },
    GenerationFrequency = Frequency.Uncommonly,
    Noise = ActorNoiseType.Roar
}
