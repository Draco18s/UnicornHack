new Creature
{
    Name = "lieutenant",
    Species = Species.Human,
    ArmorClass = 10,
    MagicResistance = 15,
    MovementRate = 10,
    Weight = 1000,
    Size = Size.Medium,
    Nutrition = 400,
    Abilities = new HashSet<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnMeleeAttack,
            Action = AbilityAction.Modifier,
            Effects = new HashSet<Effect> { new PhysicalDamage { Damage = 7 } }
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
    }
,
    SimpleProperties = new HashSet<string> { "Infravisibility", "Humanoidness", "Omnivorism" },
    InitialLevel = 10,
    PreviousStageName = "sergeant",
    NextStageName = "captain",
    GenerationFlags = GenerationFlags.NonPolymorphable,
    GenerationFrequency = Frequency.Rarely,
    Behavior = MonsterBehavior.Wandering | MonsterBehavior.Stalking | MonsterBehavior.Displacing | MonsterBehavior.GoldCollector | MonsterBehavior.WeaponCollector | MonsterBehavior.Bribeable,
    Alignment = -4,
    Noise = ActorNoiseType.Soldier
}
