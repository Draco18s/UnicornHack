using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature YellowLight = new Creature
        {
            Name = "yellow light",
            Species = Species.FloatingSphere,
            SpeciesClass = SpeciesClass.Extraplanar,
            MovementDelay = 80,
            Material = Material.Air,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Action = AbilityAction.Explosion,
                    Timeout = 1,
                    Effects = new HashSet<Effect> {new Blind {Duration = 27}}
                }
            },
            InitialLevel = 3,
            GenerationWeight = new DefaultWeight {Multiplier = 4F},
            Sex = Sex.None,
            Size = 2,
            Weight = 0,
            Agility = 2,
            Constitution = 2,
            Intelligence = 2,
            Quickness = 2,
            Strength = 2,
            Willpower = 7,
            AcidResistance = 75,
            ColdResistance = 75,
            DisintegrationResistance = 75,
            ElectricityResistance = 75,
            FireResistance = 75,
            SlimingImmune = true,
            HeadType = HeadType.None,
            UpperExtremeties = ExtremityType.None,
            LowerExtremeties = ExtremityType.None,
            RespirationType = RespirationType.None,
            LocomotionType = LocomotionType.Flying,
            InventorySize = 0,
            EyeCount = 0,
            NoiseLevel = 0,
            Infravisible = true,
            InvisibilityDetection = true,
            Mindless = true,
            NonAnimal = true
        };
    }
}
