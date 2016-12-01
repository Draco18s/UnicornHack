new CreatureVariant
{
    Name = "hell hound",
    Species = Species.Dog,
    SpeciesClass = SpeciesClass.Canine,
    PreviousStageName = "hell hound pup",
    InitialLevel = 12,
    ArmorClass = 2,
    MagicResistance = 20,
    MovementRate = 14,
    Weight = 700,
    Size = Size.Medium,
    Nutrition = 300,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 10 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Breath,
            Timeout = 1,
            Effects = new AbilityEffect[] { new FireDamage { Damage = 10 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "AnimalBody", "Infravisibility", "Handlessness", "Carnivorism", "SingularInventory" },
    ValuedProperties = new Dictionary<string, Object> { { "FireResistance", 3 } },
    GenerationFlags = GenerationFlags.HellOnly,
    GenerationFrequency = Frequency.Usually,
    Alignment = -5,
    Noise = ActorNoiseType.Bark
}
