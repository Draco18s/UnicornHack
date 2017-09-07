using System.Collections.Generic;
using UnicornHack.Effects;
using UnicornHack.Generation;

namespace UnicornHack.Data.Creatures
{
    public static partial class CreatureData
    {
        public static readonly CreatureVariant Couatl = new CreatureVariant
        {
            Name = "couatl",
            Species = Species.WingedSnake,
            SpeciesClass = SpeciesClass.Reptile | SpeciesClass.Celestial,
            MovementDelay = 120,
            Abilities =
                new HashSet<AbilityDefinition>
                {
                    new AbilityDefinition
                    {
                        Activation = AbilityActivation.OnTarget,
                        Action = AbilityAction.Bite,
                        Timeout = 1,
                        Effects = new HashSet<Effect> {new PhysicalDamage {Damage = 7}}
                    },
                    new AbilityDefinition
                    {
                        Activation = AbilityActivation.OnTarget,
                        Action = AbilityAction.Hug,
                        Timeout = 1,
                        Effects = new HashSet<Effect> {new Bind {Duration = 7}}
                    }
                },
            SimpleProperties =
                new HashSet<string>
                {
                    "flight",
                    "flight control",
                    "infravision",
                    "serpentlike body",
                    "singular inventory"
                },
            ValuedProperties =
                new Dictionary<string, object>
                {
                    {"poison resistance", 3},
                    {"venom resistance", 3},
                    {"size", 8},
                    {"physical deflection", 15},
                    {"magic resistance", 30},
                    {"weight", 900}
                },
            InitialLevel = 8,
            GenerationWeight = new BranchWeight {NotMatched = new DefaultWeight {Multiplier = 4F}, Name = "hell"},
            CorpseName = "",
            GenerationFlags = GenerationFlags.SmallGroup,
            Behavior = MonsterBehavior.AlignmentAware | MonsterBehavior.Stalking,
            Noise = ActorNoiseType.Hiss
        };
    }
}