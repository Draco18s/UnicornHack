new CreatureVariant
{
    Name = "ice troll",
    Species = Species.Troll,
    InitialLevel = 9,
    ArmorClass = 2,
    MagicResistance = 20,
    MovementRate = 10,
    Weight = 1000,
    Size = Size.Large,
    Nutrition = 350,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnMeleeAttack,
            Action = AbilityAction.Modifier,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 7 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 1 } }
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
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 7 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Infravision", "Infravisibility", "Humanoidness", "Carnivorism", "Reanimation" },
    ValuedProperties = new Dictionary<string, Object> { { "ColdResistance", 3 }, { "Regeneration", 3 } },
    GenerationFlags = GenerationFlags.NoHell,
    GenerationFrequency = Frequency.Occasionally,
    Behavior = MonsterBehavior.Stalking | MonsterBehavior.WeaponCollector,
    Alignment = -3,
    Noise = ActorNoiseType.Grunt
}