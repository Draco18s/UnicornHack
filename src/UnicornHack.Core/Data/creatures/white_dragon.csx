new CreatureVariant
{
    Name = "white dragon",
    Species = Species.Dragon,
    SpeciesClass = SpeciesClass.Reptile,
    PreviousStageName = "baby white dragon",
    InitialLevel = 15,
    ArmorClass = -1,
    MagicResistance = 20,
    MovementRate = 9,
    Weight = 4500,
    Size = Size.Gigantic,
    Nutrition = 1500,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Breath,
            Timeout = 1,
            Effects = new AbilityEffect[] { new ColdDamage { Damage = 14 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 9 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Claw,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 5 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Claw,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 5 } }
        }
    }
,
    SimpleProperties = new HashSet<string>
    {
        "Flight",
        "FlightControl",
        "InvisibilityDetection",
        "Infravision",
        "AnimalBody",
        "Handlessness",
        "Carnivorism",
        "Oviparity",
        "SingularInventory"
    }
,
    ValuedProperties = new Dictionary<string, Object> { { "ColdResistance", 3 }, { "PoisonResistance", 3 }, { "DangerAwareness", 3 }, { "ThickHide", 3 } },
    GenerationFrequency = Frequency.Uncommonly,
    Behavior = MonsterBehavior.Mountable | MonsterBehavior.GoldCollector | MonsterBehavior.GemCollector,
    Alignment = 4,
    Noise = ActorNoiseType.Roar
}