new CreatureVariant
{
    Name = "healer",
    Species = Species.Human,
    PreviousStageName = "attendant",
    InitialLevel = 10,
    ArmorClass = 10,
    MagicResistance = 10,
    MovementRate = 12,
    Weight = 1000,
    Size = Size.Medium,
    Nutrition = 400,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Spell,
            Timeout = 1,
            Effects = new AbilityEffect[] { new ScriptedEffect { Script = "DivineSpell" } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Infravisibility", "Humanoidness", "Herbivorism" },
    ValuedProperties = new Dictionary<string, Object> { { "PoisonResistance", 3 } },
    GenerationFlags = GenerationFlags.NonGenocidable | GenerationFlags.NonPolymorphable,
    Behavior = MonsterBehavior.Peaceful | MonsterBehavior.GoldCollector | MonsterBehavior.MagicUser,
    Alignment = 3,
    Noise = ActorNoiseType.Speach
}