using UnicornHack.Systems.Effects;

namespace UnicornHack.Generation.Effects
{
    public class Disintegrate : DamageEffect
    {
        protected override void ConfigureEffect(EffectComponent effect)
        {
            base.ConfigureEffect(effect);

            effect.EffectType = EffectType.Disintegrate;
        }
    }
}
