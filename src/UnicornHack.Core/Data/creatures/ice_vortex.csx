new CreatureVariant
{
    Name = "ice vortex",
    Species = Species.Vortex,
    SpeciesClass = SpeciesClass.Extraplanar,
    CorpseVariantName = "",
    InitialLevel = 5,
    ArmorClass = 2,
    MagicResistance = 30,
    MovementRate = 20,
    Size = Size.Huge,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Touch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new Engulf { Duration = 4 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnDigestion,
            Action = AbilityAction.Digestion,
            Timeout = 1,
            Effects = new AbilityEffect[] { new ColdDamage { Damage = 3 } }
        }
,
        new Ability { Activation = AbilityActivation.OnMeleeHit, Effects = new AbilityEffect[] { new ColdDamage { Damage = 3 } } },
        new Ability { Activation = AbilityActivation.OnRangedHit, Effects = new AbilityEffect[] { new ColdDamage { Damage = 3 } } }
    }
,
    SimpleProperties = new HashSet<string>
    {
        "SleepResistance",
        "Flight",
        "FlightControl",
        "Infravisibility",
        "NonAnimal",
        "NonSolidBody",
        "Breathlessness",
        "Limblessness",
        "Eyelessness",
        "Headlessness",
        "Mindlessness",
        "Asexuality",
        "StoningResistance",
        "SlimingResistance",
        "SicknessResistance"
    }
,
    ValuedProperties = new Dictionary<string, Object> { { "ColdResistance", 3 }, { "PoisonResistance", 3 }, { "VenomResistance", 3 } },
    GenerationFlags = GenerationFlags.NoHell,
    GenerationFrequency = Frequency.Commonly
}
