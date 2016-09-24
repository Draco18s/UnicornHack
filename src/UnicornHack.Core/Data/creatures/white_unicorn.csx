new CreatureVariant
{
    InitialLevel = 4,
    ArmorClass = 2,
    MagicResistance = 70,
    GenerationFrequency = Frequency.Usually,
    Behavior = MonsterBehavior.AlignmentAware | MonsterBehavior.RangedPeaceful | MonsterBehavior.Wandering | MonsterBehavior.GemCollector,
    Alignment = 7,
    Noise = ActorNoiseType.Neigh,
    Name = "white unicorn",
    Species = Species.Unicorn,
    SpeciesClass = SpeciesClass.Quadrupedal,
    MovementRate = 24,
    Size = Size.Large,
    Weight = 1300,
    Nutrition = 700,
    SimpleProperties = new HashSet<string> { "AnimalBody", "Infravisibility", "Handlessness", "Herbivorism" },
    ValuedProperties = new Dictionary<string, Object> { { "PoisonResistance", 3 }, { "VenomResistance", 3 } },
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.Targetted,
            Action = AbilityAction.Headbutt,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 6 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.Targetted,
            Action = AbilityAction.Kick,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 3 } }
        }
    }
}
