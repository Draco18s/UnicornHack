using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature WoodNymph = new Creature
        {
            Name = "wood nymph",
            Species = Species.Nymph,
            SpeciesClass = SpeciesClass.Fey,
            MovementDelay = 100,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Range = 1,
                    Action = AbilityAction.Touch,
                    Cooldown = 100,
                    Effects = new HashSet<Effect>
                        {new Sedate {Duration = EffectDuration.UntilTimeout, DurationAmount = "2"}}
                },
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Range = 1,
                    Action = AbilityAction.Touch,
                    Cooldown = 100,
                    Effects = new HashSet<Effect>
                    {
                        new StealItem()
                    }
                }
            },
            InitialLevel = 3,
            GenerationWeight = new DefaultWeight {Multiplier = 4F},
            Behavior = AIBehavior.WeaponCollector,
            Noise = ActorNoiseType.Seduction,
            Sex = Sex.Female,
            Weight = 600,
            Perception = 2,
            Might = 3,
            Speed = 5,
            Focus = 5,
            MagicResistance = 10,
            TorsoType = TorsoType.Humanoid,
            UpperExtremities = ExtremityType.GraspingFingers,
            LowerExtremities = ExtremityType.Fingers,
            LocomotionType = LocomotionType.Walking | LocomotionType.Teleportation,
            Infravisible = true
        };
    }
}
