new CreatureVariant
{
    Name = "chameleon",
    Species = Species.Lizard,
    SpeciesClass = SpeciesClass.Reptile | SpeciesClass.ShapeChanger,
    InitialLevel = 5,
    ArmorClass = 6,
    MagicResistance = 10,
    MovementRate = 6,
    Weight = 50,
    Size = Size.Small,
    Nutrition = 50,
    Abilities = new List<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new AbilityEffect[] { new PhysicalDamage { Damage = 6 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "Handlessness", "PolymorphControl", "Oviparity", "Carnivorism", "SingularInventory" },
    GenerationFlags = GenerationFlags.NonPolymorphable,
    GenerationFrequency = Frequency.Uncommonly
}