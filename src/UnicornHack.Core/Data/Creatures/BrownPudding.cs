using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature BrownPudding = new Creature
        {
            Name = "brown pudding",
            Species = Species.Pudding,
            MovementDelay = 400,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Range = 1,
                    Action = AbilityAction.Touch,
                    Cooldown = 100,
                    Effects = new HashSet<Effect> {new Blight {Damage = "30"}}
                },
                new Ability
                {
                    Activation = ActivationType.OnMeleeHit, Effects = new HashSet<Effect> {new Blight {Damage = "30"}}
                }
            },
            InitialLevel = 5,
            Sex = Sex.None,
            Weight = 512,
            Perception = 3,
            Might = 2,
            Speed = 3,
            Focus = 8,
            Armor = 1,
            AcidResistance = 75,
            ColdResistance = 75,
            ElectricityResistance = 75,
            StoningImmune = true,
            HeadType = HeadType.None,
            TorsoType = TorsoType.Amorphic,
            UpperExtremities = ExtremityType.None,
            LowerExtremities = ExtremityType.None,
            RespirationType = RespirationType.None,
            EyeCount = 0,
            NoiseLevel = 0,
            Mindless = true,
            NonAnimal = true,
            Reanimation = true
        };
    }
}
