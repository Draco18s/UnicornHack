new Creature
{
    Name = "master lich",
    Species = Species.Lich,
    SpeciesClass = SpeciesClass.Undead,
    ArmorClass = -4,
    MagicResistance = 90,
    MovementRate = 9,
    Weight = 600,
    Size = Size.Medium,
    Nutrition = 50,
    Abilities = new HashSet<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Touch,
            Timeout = 1,
            Effects = new HashSet<Effect> { new ColdDamage { Damage = 10 } }
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
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new HashSet<Effect> { new Infect { } } },
        new Ability { Activation = AbilityActivation.OnConsumption, Effects = new HashSet<Effect> { new PoisonDamage { Damage = 14 } } }
    }
,
    SimpleProperties = new HashSet<string> { "SleepResistance", "Infravision", "Humanoidness", "Breathlessness", "SicknessResistance" },
    ValuedProperties = new Dictionary<string, Object> { { "FireResistance", 3 }, { "ColdResistance", 3 }, { "PoisonResistance", 3 }, { "VenomResistance", 3 }, { "Regeneration", 3 } },
    InitialLevel = 17,
    PreviousStageName = "demilich",
    NextStageName = "arch-lich",
    CorpseName = "",
    GenerationFlags = GenerationFlags.HellOnly,
    GenerationFrequency = Frequency.Rarely,
    Behavior = MonsterBehavior.MagicUser | MonsterBehavior.Covetous,
    Alignment = -15,
    Noise = ActorNoiseType.Mumble
}
