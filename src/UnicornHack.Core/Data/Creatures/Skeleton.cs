using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature Skeleton = new Creature
        {
            Name = "skeleton",
            Species = Species.Skeleton,
            SpeciesClass = SpeciesClass.Undead,
            MovementDelay = 200,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.OnMeleeAttack,
                    Action = AbilityAction.Modifier,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = "30"}}
                },
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Range = 1,
                    Action = AbilityAction.Punch,
                    Cooldown = 100,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = "10"}}
                }
            },
            InitialLevel = 3,
            GenerationWeight = new DefaultWeight {Multiplier = 3F},
            GenerationFlags = GenerationFlags.SmallGroup,
            Behavior = AIBehavior.WeaponCollector,
            Weight = 300,
            Perception = 2,
            Might = 2,
            Speed = 2,
            Focus = 6,
            PhysicalResistance = 20,
            ColdResistance = 75,
            StoningImmune = true,
            TorsoType = TorsoType.Humanoid,
            UpperExtremities = ExtremityType.GraspingFingers,
            LowerExtremities = ExtremityType.Fingers,
            RespirationType = RespirationType.None,
            Infravision = true,
            Mindless = true
        };
    }
}
