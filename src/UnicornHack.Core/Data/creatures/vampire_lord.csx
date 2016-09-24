new CreatureVariant
{
    InitialLevel = 12,
    MagicResistance = 40,
    GenerationFrequency = Frequency.Rarely,
    Behavior = MonsterBehavior.Stalking | MonsterBehavior.WeaponCollector,
    Alignment = -9,
    Noise = ActorNoiseType.Vampire,
    CorpseVariantName = "",
    PreviousStageName = "vampire",
    NextStageName = "vampire mage",
    Name = "vampire lord",
    Species = Species.Vampire,
    SpeciesClass = SpeciesClass.ShapeChanger | SpeciesClass.Undead,
    MovementRate = 14,
    Size = Size.Medium,
    Weight = 1000,
    Nutrition = 400,
    SimpleProperties = new HashSet<string> { "SleepResistance", "Flight", "FlightControl", "Infravision", "Humanoidness", "Breathlessness", "Maleness" },
    ValuedProperties = new Dictionary<string, Object> { { "PoisonResistance", 3 }, { "SicknessResistance", 3 }, { "Regeneration", 3 } },
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnMeleeAttack,
            Action = AbilityAction.Modifier,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 4 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.Targetted,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 1 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.Targetted,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new DrainLife { Amount = 4 } }
        }
,
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new AbilityEffect[] { new Infect { } } }
    }
}
