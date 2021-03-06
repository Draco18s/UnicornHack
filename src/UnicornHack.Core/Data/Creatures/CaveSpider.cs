using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature CaveSpider = new Creature
        {
            Name = "cave spider",
            Species = Species.Spider,
            SpeciesClass = SpeciesClass.Vermin,
            MovementDelay = 100,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Range = 1,
                    Action = AbilityAction.Bite,
                    Cooldown = 100,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = "10"}}
                }
            },
            InitialLevel = 1,
            GenerationWeight = new DefaultWeight {Multiplier = 6F},
            GenerationFlags = GenerationFlags.SmallGroup,
            Size = 2,
            Weight = 50,
            Perception = 1,
            Might = 3,
            Speed = 3,
            Focus = 1,
            Armor = 3,
            TorsoType = TorsoType.Quadruped,
            UpperExtremities = ExtremityType.None,
            LowerExtremities = ExtremityType.Claws,
            VisibilityLevel = 2,
            Clingy = true
        };
    }
}
