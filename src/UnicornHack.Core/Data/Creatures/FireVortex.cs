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
                    Range = 1,
                    Action = AbilityAction.Touch,
                    Cooldown = 100,
                    Effects = new HashSet<Effect>
                        {new Engulf {Duration = EffectDuration.UntilTimeout, DurationAmount = "4"}}
                },
                new Ability
                {
                    Activation = ActivationType.OnDigestion,
                    Action = AbilityAction.Digestion,
                    Cooldown = 100,
                    Effects = new HashSet<Effect> {new Burn {Damage = "50"}}
                },
                new Ability
                {
                    Activation = ActivationType.OnMeleeHit, Effects = new HashSet<Effect> {new Burn {Damage = "50"}}
                },
                new Ability
                {
                    Activation = ActivationType.OnRangedHit, Effects = new HashSet<Effect> {new Burn {Damage = "50"}}
                }
            },
            InitialLevel = 8,
            GenerationWeight = new DefaultWeight {Multiplier = 5F},
            Sex = Sex.None,
            Size = 16,
            Weight = 0,
            Perception = 5,
            Might = 4,
            Speed = 5,
            Focus = 10,
            Armor = 4,
            MagicResistance = 15,
            AcidResistance = 75,
            FireResistance = 75,
            SlimingImmune = true,
            StoningImmune = true,
            HeadType = HeadType.None,
            UpperExtremities = ExtremityType.None,
            LowerExtremities = ExtremityType.None,
            RespirationType = RespirationType.None,
            LocomotionType = LocomotionType.Flying,
            EyeCount = 0,
            Infravisible = true,
            Mindless = true,
            NonAnimal = true
        };
    }
}
