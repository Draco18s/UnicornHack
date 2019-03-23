using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature OrcZombie = new Creature
        {
            Name = "orc zombie",
            Species = Species.Orc,
            SpeciesClass = SpeciesClass.Undead,
            MovementDelay = 133,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Range = 1,
                    Action = AbilityAction.Punch,
                    Cooldown = 100,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = "30"}}
                }
            },
            InitialLevel = 3,
            GenerationWeight = new DefaultWeight {Multiplier = 2F},
            GenerationFlags = GenerationFlags.SmallGroup,
            Behavior = AIBehavior.Stalking,
            Noise = ActorNoiseType.Moan,
            Weight = 1000,
            Perception = 2,
            Might = 2,
            Speed = 2,
            Focus = 6,
            Armor = 2,
            ColdResistance = 75,
            TorsoType = TorsoType.Humanoid,
            UpperExtremities = ExtremityType.GraspingFingers,
            LowerExtremities = ExtremityType.Fingers,
            RespirationType = RespirationType.None,
            Infravision = true,
            Mindless = true
        };
    }
}
