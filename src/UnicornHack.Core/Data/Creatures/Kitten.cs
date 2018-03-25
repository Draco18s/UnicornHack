using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature Kitten = new Creature
        {
            Name = "kitten",
            Species = Species.Cat,
            SpeciesClass = SpeciesClass.Feline,
            MovementDelay = 66,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Action = AbilityAction.Bite,
                    Timeout = 1,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = 30}}
                }
            },
            InitialLevel = 2,
            GenerationWeight = new DefaultWeight {Multiplier = 7F},
            NextStageName = "housecat",
            Behavior = AIBehavior.Domesticable | AIBehavior.Wandering,
            Noise = ActorNoiseType.Mew,
            Size = 2,
            Weight = 150,
            Agility = 2,
            Constitution = 2,
            Intelligence = 2,
            Quickness = 2,
            Strength = 2,
            Willpower = 2,
            PhysicalDeflection = 14,
            TorsoType = TorsoType.Quadruped,
            UpperExtremeties = ExtremityType.None,
            LowerExtremeties = ExtremityType.Claws,
            InventorySize = 1,
            Infravisible = true
        };
    }
}
