new CreatureVariant
{
    Name = "Juiblex",
    Species = Species.DemonMajor,
    SpeciesClass = SpeciesClass.Demon,
    CorpseVariantName = "",
    InitialLevel = 50,
    ArmorClass = -7,
    MagicResistance = 65,
    MovementRate = 3,
    Weight = 1500,
    Size = Size.Large,
    Nutrition = 400,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Touch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new Engulf { Duration = 20 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnDigestion,
            Action = AbilityAction.Digestion,
            Timeout = 1,
            Effects = new AbilityEffect[] { new AcidDamage { Damage = 22 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Spit,
            Timeout = 1,
            Effects = new AbilityEffect[] { new AcidDamage { Damage = 10 } }
        }
,
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new AbilityEffect[] { new PoisonDamage { Damage = 10 } } },
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new AbilityEffect[] { new AcidDamage { Damage = 10 } } }
    }
,
    SimpleProperties = new HashSet<string>
    {
        "Amphibiousness",
        "Flight",
        "FlightControl",
        "Amorphism",
        "Headlessness",
        "Infravision",
        "InvisibilityDetection",
        "Maleness",
        "StoningResistance",
        "SicknessResistance"
    }
,
    ValuedProperties = new Dictionary<string, Object> { { "FireResistance", 3 }, { "PoisonResistance", 3 }, { "AcidResistance", 3 } },
    GenerationFlags = GenerationFlags.NonGenocidable | GenerationFlags.NonPolymorphable | GenerationFlags.HellOnly,
    Behavior = MonsterBehavior.RangedPeaceful | MonsterBehavior.Stalking | MonsterBehavior.Covetous,
    Alignment = -15,
    Noise = ActorNoiseType.Gurgle
}