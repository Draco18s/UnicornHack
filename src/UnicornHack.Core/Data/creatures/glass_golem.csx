new CreatureVariant
{
    Name = "glass golem",
    Species = Species.Golem,
    CorpseVariantName = "",
    InitialLevel = 16,
    ArmorClass = 4,
    MagicResistance = 50,
    MovementRate = 6,
    Weight = 1800,
    Size = Size.Large,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 9 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 9 } }
        }
    }
,
    SimpleProperties = new HashSet<string>
    {
        "Reflection",
        "SleepResistance",
        "NonAnimal",
        "Breathlessness",
        "Mindlessness",
        "Humanoidness",
        "Asexuality",
        "StoningResistance",
        "SlimingResistance",
        "SicknessResistance"
    }
,
    ValuedProperties = new Dictionary<string, Object>
    {
        {
            "AcidResistance",
            3
        },
        {
            "ColdResistance",
            3
        },
        {
            "FireResistance",
            3
        },
        {
            "ElectricityResistance",
            3
        },
        {
            "PoisonResistance",
            3
        },
        {
            "VenomResistance",
            3
        },
        {
            "ThickHide",
            3
        },
        {
            "MaxHP",
            60
        }
    }
,
    GenerationFrequency = Frequency.Rarely
}