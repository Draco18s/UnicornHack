using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature Shrieker = new Creature
        {
            Name = "shrieker",
            Species = Species.Fungus,
            MovementDelay = 1200,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Range = 20,
                    Action = AbilityAction.Scream,
                    Cooldown = 100,
                    Effects = new HashSet<Effect>
                        {new Deafen {Duration = EffectDuration.UntilTimeout, DurationAmount = "3"}}
                }
            },
            InitialLevel = 3,
            GenerationWeight = new DefaultWeight {Multiplier = 2F},
            Sex = Sex.None,
            Size = 2,
            Weight = 100,
            Perception = 2,
            Might = 2,
            Speed = 2,
            Focus = 6,
            Armor = 1,
            HeadType = HeadType.None,
            UpperExtremities = ExtremityType.None,
            LowerExtremities = ExtremityType.None,
            RespirationType = RespirationType.None,
            InventorySize = 0,
            EyeCount = 0,
            Mindless = true,
            NonAnimal = true
        };
    }
}
