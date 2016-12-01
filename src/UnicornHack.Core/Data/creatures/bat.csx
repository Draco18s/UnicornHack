new CreatureVariant
{
    Name = "bat",
    Species = Species.Bat,
    SpeciesClass = SpeciesClass.Bird,
    NextStageName = "giant bat",
    InitialLevel = 1,
    ArmorClass = 8,
    MovementRate = 22,
    Weight = 50,
    Size = Size.Tiny,
    Nutrition = 20,
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
    SimpleProperties = new HashSet<string> { "Flight", "FlightControl", "Infravisibility", "AnimalBody", "Handlessness", "Carnivorism", "SingularInventory" },
    ValuedProperties = new Dictionary<string, Object> { { "PoisonResistance", 3 }, { "Stealthiness", 3 } },
    GenerationFlags = GenerationFlags.SmallGroup,
    GenerationFrequency = Frequency.Commonly,
    Behavior = MonsterBehavior.Wandering,
    Noise = ActorNoiseType.Sqeek
}