using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly Creature Demilich = new Creature
        {
            Name = "demilich",
            Species = Species.Lich,
            SpeciesClass = SpeciesClass.Undead,
            MovementDelay = 133,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Range = 1,
                    Action = AbilityAction.Touch,
                    Cooldown = 100,
                    Effects = new HashSet<Effect> {new Freeze {Damage = "70"}}
                },
                new Ability
                {
                    Activation = ActivationType.Targeted,
                    Range = 20,
                    Action = AbilityAction.Spell,
                    Cooldown = 100,
                    Effects = new HashSet<Effect> {new ScriptedEffect {Script = "ArcaneSpell"}}
                }
            },
            InitialLevel = 14,
            PreviousStageName = "lich",
            NextStageName = "master lich",
            Behavior = AIBehavior.MagicUser,
            Noise = ActorNoiseType.Mumble,
            Weight = 600,
            Perception = 8,
            Might = 8,
            Speed = 8,
            Focus = 12,
            Regeneration = 3,
            Armor = 6,
            MagicResistance = 30,
            ColdResistance = 75,
            TorsoType = TorsoType.Humanoid,
            UpperExtremities = ExtremityType.GraspingFingers,
            LowerExtremities = ExtremityType.Fingers,
            RespirationType = RespirationType.None,
            Infravision = true
        };
    }
}
