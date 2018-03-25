using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature FireVortex = new Creature
        {
            Name = "fire vortex",
            Species = Species.Vortex,
            SpeciesClass = SpeciesClass.Extraplanar,
            MovementDelay = 54,
            Material = Material.Air,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Action = AbilityAction.Touch,
                    Timeout = 1,
                    Effects = new HashSet<Effect> {new Engulf {Duration = 4}}
                },
                new Ability
                {
                    Activation = ActivationType.OnDigestion,
                    Action = AbilityAction.Digestion,
                    Timeout = 1,
                    Effects = new HashSet<Effect> {new Burn {Damage = 50}}
                },
                new Ability
                {
                    Activation = ActivationType.OnPhysicalMeleeHit,
                    Effects = new HashSet<Effect> {new Burn {Damage = 50}}
                },
                new Ability
                {
                    Activation = ActivationType.OnPhysicalRangedHit,
                    Effects = new HashSet<Effect> {new Burn {Damage = 50}}
                }
            },
            InitialLevel = 8,
            GenerationWeight = new DefaultWeight {Multiplier = 5F},
            Sex = Sex.None,
            Size = 16,
            Weight = 0,
            Agility = 5,
            Constitution = 5,
            Intelligence = 5,
            Quickness = 5,
            Strength = 5,
            Willpower = 10,
            MagicResistance = 30,
            PhysicalDeflection = 18,
            AcidResistance = 75,
            FireResistance = 75,
            SlimingImmune = true,
            StoningImmune = true,
            HeadType = HeadType.None,
            UpperExtremeties = ExtremityType.None,
            LowerExtremeties = ExtremityType.None,
            RespirationType = RespirationType.None,
            LocomotionType = LocomotionType.Flying,
            EyeCount = 0,
            Infravisible = true,
            Mindless = true,
            NonAnimal = true
        };
    }
}
