using UnicornHack.Primitives;
using UnicornHack.Systems.Effects;

namespace UnicornHack.Generation.Effects
{
    public class Soak : DamageEffect
    {
        protected override void ConfigureEffect(EffectComponent effect)
        {
            base.ConfigureEffect(effect);

            effect.EffectType = EffectType.Soak;
        }
    }
}
