new CreatureVariant
{
    Name = "golden naga",
    Species = Species.Naga,
    SpeciesClass = SpeciesClass.Aberration,
    PreviousStageName = "golden naga hatchling",
    InitialLevel = 10,
    ArmorClass = 2,
    MagicResistance = 70,
    MovementRate = 14,
    Weight = 1500,
    Size = Size.Huge,
    Nutrition = 600,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 7 } }
        }
,
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Spell,
            Timeout = 1,
            Effects = new AbilityEffect[] { new MagicalDamage { Damage = 14 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Infravision", "SerpentlikeBody", "Limblessness", "Carnivorism", "Oviparity", "SingularInventory", "StoningResistance" },
    ValuedProperties = new Dictionary<string, Object> { { "AcidResistance", 3 }, { "PoisonResistance", 3 }, { "VenomResistance", 3 }, { "ThickHide", 3 } },
    GenerationFrequency = Frequency.Uncommonly,
    Alignment = 5,
    Noise = ActorNoiseType.Hiss
}
