new Creature
{
    Name = "coyote",
    Species = Species.Dog,
    SpeciesClass = SpeciesClass.Canine,
    ArmorClass = 7,
    MovementRate = 12,
    Weight = 300,
    Size = Size.Small,
    Nutrition = 250,
    Abilities = new HashSet<Ability>
    {
        new Ability
        {
            Activation = AbilityActivation.OnTarget,
            Action = AbilityAction.Bite,
            Timeout = 1,
            Effects = new HashSet<Effect> { new PhysicalDamage { Damage = 1 } }
        }
    }
,
    SimpleProperties = new HashSet<string> { "AnimalBody", "Infravisibility", "Handlessness", "Carnivorism", "SingularInventory" },
    InitialLevel = 1,
    GenerationFrequency = Frequency.Usually,
    Noise = ActorNoiseType.Bark
}
