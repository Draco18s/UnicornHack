new Creature
{
    Name = "Orcus",
    Species = Species.DemonMajor,
    SpeciesClass = SpeciesClass.Demon,
    ArmorClass = -6,
    MagicResistance = 85,
    MovementRate = 9,
    Weight = 2500,
    Size = Size.Huge,
    Nutrition = 400,
    Abilities = new HashSet<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnMeleeAttack,
            Action = AbilityAction.Modifier,
            Effects = new HashSet<Effect> { new PhysicalDamage { Damage = 10 } }
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
            Action = AbilityAction.Claw,
            Timeout = 1,
            Effects = new HashSet<Effect> { new PhysicalDamage { Damage = 7 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Headbutt,
            Timeout = 1,
            Effects = new HashSet<Effect> { new VenomDamage { Damage = 5 } }
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
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new HashSet<Effect> { new PoisonDamage { Damage = 13 } } },
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new HashSet<Effect> { new Infect { Strength = 13 } } }
    }
,
    SimpleProperties = new HashSet<string>
    {
        "Flight",
        "FlightControl",
        "Infravisibility",
        "Infravision",
        "InvisibilityDetection",
        "Humanoidness",
        "Maleness",
        "SicknessResistance"
    }
,
    ValuedProperties = new Dictionary<string, Object> { { "FireResistance", 3 }, { "PoisonResistance", 3 }, { "VenomResistance", 3 } },
    InitialLevel = 66,
    CorpseName = "",
    GenerationFlags = GenerationFlags.NonGenocidable | GenerationFlags.NonPolymorphable | GenerationFlags.HellOnly,
    Behavior = MonsterBehavior.RangedPeaceful | MonsterBehavior.Stalking | MonsterBehavior.WeaponCollector | MonsterBehavior.MagicUser | MonsterBehavior.Covetous,
    Alignment = -20,
    Noise = ActorNoiseType.Grunt
}
