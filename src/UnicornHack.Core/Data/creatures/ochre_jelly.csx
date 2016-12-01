new CreatureVariant
{
    Name = "ochre jelly",
    Species = Species.Jelly,
    InitialLevel = 6,
    ArmorClass = 8,
    MagicResistance = 20,
    MovementRate = 3,
    Weight = 100,
    Size = Size.Small,
    Nutrition = 20,
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
            Effects = new AbilityEffect[] { new AcidDamage { Damage = 10 } }
        }
,
        new Ability { Activation = AbilityActivation.OnMeleeHit, Effects = new AbilityEffect[] { new AcidDamage { Damage = 7 } } },
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new AbilityEffect[] { new AcidDamage { Damage = 7 } } }
    }
,
    SimpleProperties = new HashSet<string>
    {
        "SleepResistance",
        "Breathlessness",
        "Amorphism",
        "NonAnimal",
        "Eyelessness",
        "Limblessness",
        "Headlessness",
        "Mindlessness",
        "Asexuality",
        "NoInventory",
        "StoningResistance"
    }
,
    ValuedProperties = new Dictionary<string, Object> { { "PoisonResistance", 3 }, { "VenomResistance", 3 }, { "AcidResistance", 3 }, { "Stealthiness", 3 } },
    GenerationFrequency = Frequency.Commonly
}