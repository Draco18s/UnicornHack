new Creature
{
    Name = "Nalzok",
    Species = Species.DemonMajor,
    SpeciesClass = SpeciesClass.Demon,
    ArmorClass = -2,
    MagicResistance = 75,
    MovementDelay = 100,
    Weight = 2500,
    Size = Size.Huge,
    Nutrition = 400,
    Abilities = new HashSet<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnMeleeAttack,
            Action = AbilityAction.Modifier,
            Effects = new HashSet<Effect> { new PhysicalDamage { Damage = 20 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new HashSet<Effect> { new PhysicalDamage { Damage = 1 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new HashSet<Effect> { new PhysicalDamage { Damage = 1 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Spell,
            Timeout = 1,
            Effects = new HashSet<Effect> { new ScriptedEffect { Script = "ArcaneSpell" } }
        }
,
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new HashSet<Effect> { new PoisonDamage { Damage = 5 } } }
    }
,
    SimpleProperties = new HashSet<string>
    {
        "Flight",
        "FlightControl",
        "Infravision",
        "InvisibilityDetection",
        "Infravisibility",
        "Maleness",
        "Humanoidness",
        "StoningResistance",
        "SicknessResistance"
    }
,
    ValuedProperties = new Dictionary<string, Object> { { "FireResistance", 3 }, { "PoisonResistance", 3 } },
    InitialLevel = 16,
    CorpseName = "",
    GenerationFlags = GenerationFlags.NonGenocidable | GenerationFlags.NonPolymorphable,
    GenerationFrequency = Frequency.Rarely,
    Behavior = MonsterBehavior.RangedPeaceful | MonsterBehavior.Stalking | MonsterBehavior.WeaponCollector | MonsterBehavior.MagicUser | MonsterBehavior.Covetous,
    Alignment = -16,
    Noise = ActorNoiseType.Quest
}
