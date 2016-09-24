new CreatureVariant
{
    InitialLevel = 1,
    ArmorClass = 9,
    GenerationFrequency = Frequency.Uncommonly,
    Name = "brown mold",
    Species = Species.Fungus,
    Size = Size.Small,
    Weight = 50,
    Nutrition = 30,
    SimpleProperties = new HashSet<string>
    {
        "SleepResistance",
        "DecayResistance",
        "Breathlessness",
        "NonAnimal",
        "Eyelessness",
        "Limblessness",
        "Headlessness",
        "Mindlessness",
        "Asexuality",
        "NoInventory"
    }
,
    ValuedProperties = new Dictionary<string, Object> { { "ColdResistance", 3 }, { "PoisonResistance", 3 }, { "VenomResistance", 3 }, { "Stealthiness", 3 } },
    Abilities = new List<Ability>
    {
        new Ability { Activation = AbilityActivation.OnMeleeHit, Effects = new AbilityEffect[] { new ColdDamage { Damage = 3 } } },
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new AbilityEffect[] { new ColdDamage { Damage = 2 } } }
    }
}
