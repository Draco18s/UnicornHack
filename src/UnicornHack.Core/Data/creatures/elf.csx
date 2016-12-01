new CreatureVariant
{
    Name = "elf",
    Species = Species.Elf,
    InitialLevel = 4,
    ArmorClass = 10,
    MagicResistance = 10,
    MovementRate = 12,
    Weight = 800,
    Size = Size.Medium,
    Nutrition = 350,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnMeleeAttack,
            Action = AbilityAction.Modifier,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 5 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 1 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "SleepResistance", "InvisibilityDetection", "Infravision", "Infravisibility", "Humanoidness", "Omnivorism" },
    GenerationFlags = GenerationFlags.NonPolymorphable,
    Behavior = MonsterBehavior.AlignmentAware | MonsterBehavior.WeaponCollector,
    Alignment = 3,
    Noise = ActorNoiseType.Speach
}