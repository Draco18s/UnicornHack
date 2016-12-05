new Creature
{
    Name = "black naga hatchling",
    Species = Species.Naga,
    SpeciesClass = SpeciesClass.Aberration,
    ArmorClass = 6,
    MovementRate = 10,
    Weight = 500,
    Size = Size.Medium,
    Nutrition = 200,
    Abilities = new HashSet<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new HashSet<Effect> { new PhysicalDamage { Damage = 2 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Infravision", "SerpentlikeBody", "Limblessness", "Carnivorism", "SingularInventory", "StoningResistance" },
    ValuedProperties = new Dictionary<string, Object> { { "AcidResistance", 3 }, { "PoisonResistance", 3 }, { "VenomResistance", 3 }, { "ThickHide", 3 } },
    InitialLevel = 3,
    NextStageName = "black naga",
    GenerationFrequency = Frequency.Uncommonly,
    Noise = ActorNoiseType.Hiss
}
