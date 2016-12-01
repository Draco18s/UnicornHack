new CreatureVariant
{
    Name = "red naga hatchling",
    Species = Species.Naga,
    SpeciesClass = SpeciesClass.Aberration,
    NextStageName = "red naga",
    InitialLevel = 3,
    ArmorClass = 6,
    MovementRate = 10,
    Weight = 500,
    Size = Size.Medium,
    Nutrition = 200,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 2 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Infravision", "SerpentlikeBody", "Limblessness", "Carnivorism", "SingularInventory", "SlimingResistance" },
    ValuedProperties = new Dictionary<string, Object> { { "FireResistance", 3 }, { "PoisonResistance", 3 }, { "VenomResistance", 3 }, { "ThickHide", 3 } },
    GenerationFrequency = Frequency.Uncommonly,
    Noise = ActorNoiseType.Hiss
}