new CreatureVariant
{
    Name = "fire elemental",
    Species = Species.Elemental,
    SpeciesClass = SpeciesClass.Extraplanar,
    CorpseVariantName = "",
    InitialLevel = 8,
    ArmorClass = 2,
    MagicResistance = 30,
    MovementRate = 12,
    Size = Size.Huge,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new FireDamage { Damage = 10 } }
        }
,
        new Ability { Activation = AbilityActivation.OnMeleeHit, Effects = new AbilityEffect[] { new FireDamage { Damage = 3 } } }
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
    ValuedProperties = new Dictionary<string, Object> { { "WaterWeakness", 3 }, { "FireResistance", 3 }, { "PoisonResistance", 3 }, { "VenomResistance", 3 } },
    GenerationFrequency = Frequency.Uncommonly
}
