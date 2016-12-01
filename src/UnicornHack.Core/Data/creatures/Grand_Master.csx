new CreatureVariant
{
    Name = "Grand Master",
    Species = Species.Human,
    InitialLevel = 16,
    ArmorClass = 6,
    MagicResistance = 50,
    MovementRate = 12,
    Weight = 1000,
    Size = Size.Medium,
    Nutrition = 400,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Punch,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 4 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Kick,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 4 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Infravisibility", "Humanoidness", "Maleness", "Herbivorism", "StoningResistance" },
    GenerationFlags = GenerationFlags.NonGenocidable | GenerationFlags.NonPolymorphable,
    Behavior = MonsterBehavior.Peaceful | MonsterBehavior.MagicUser,
    Noise = ActorNoiseType.Quest
}