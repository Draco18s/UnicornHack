using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature HornedDevil = new Creature
        {
            Name = "horned devil",
            Species = Species.DemonMajor,
            SpeciesClass = SpeciesClass.Demon,
            MovementDelay = 133,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.OnPhysicalMeleeAttack,
                    Action = AbilityAction.Modifier,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = 20}}
                },
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Action = AbilityAction.Punch,
                    Timeout = 1,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = 10}}
                },
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Action = AbilityAction.Claw,
                    Timeout = 1,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = 20}}
                },
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Action = AbilityAction.Bite,
                    Timeout = 1,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = 40}}
                },
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Action = AbilityAction.Headbutt,
                    Timeout = 1,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = 20}}
                }
            },
            InitialLevel = 6,
            GenerationWeight = new BranchWeight
            {
                Matched = new DefaultWeight {Multiplier = 2F},
                Name = "hell"
            },
            NextStageName = "barbed devil",
            GenerationFlags = GenerationFlags.NonGenocidable,
            Behavior = AIBehavior.Stalking | AIBehavior.WeaponCollector,
            Weight = 1000,
            Agility = 4,
            Constitution = 4,
            Intelligence = 4,
            Quickness = 4,
            Strength = 4,
            Willpower = 4,
            MagicResistance = 50,
            PhysicalDeflection = 25,
            FireResistance = 75,
            Infravisible = true,
            Infravision = true
        };
    }
}
