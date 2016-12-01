new CreatureVariant
{
    Name = "vrock",
    Species = Species.DemonMajor,
    SpeciesClass = SpeciesClass.Demon,
    CorpseVariantName = "",
    PreviousStageName = "erinyes",
    NextStageName = "pit fiend",
    InitialLevel = 8,
    MagicResistance = 50,
    MovementRate = 12,
    Weight = 1200,
    Size = Size.Large,
    Nutrition = 400,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Claw,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 7 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Claw,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 7 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 3 } }
        }
,
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new AbilityEffect[] { new PoisonDamage { Damage = 3 } } }
    }
,
    SimpleProperties = new HashSet<string> { "Flight", "FlightControl", "Infravision", "Infravisibility", "Humanoidness", "SicknessResistance" },
    ValuedProperties = new Dictionary<string, Object> { { "FireResistance", 3 }, { "PoisonResistance", 3 } },
    GenerationFlags = GenerationFlags.NonGenocidable | GenerationFlags.SmallGroup | GenerationFlags.HellOnly,
    GenerationFrequency = Frequency.Uncommonly,
    Behavior = MonsterBehavior.Stalking,
    Alignment = -9
}