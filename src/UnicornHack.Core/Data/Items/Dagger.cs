using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Items
{
    public static partial class ItemData
    {
        public static readonly Item Dagger = new Item
        {
            Name = "dagger",
            Type = ItemType.WeaponMeleeShort,
            Material = Material.Steel,
            Weight = 5,
            EquipableSizes = SizeCategory.Tiny | SizeCategory.Small,
            EquipableSlots = EquipmentSlot.GraspSingleExtremity | EquipmentSlot.GraspBothExtremities,
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.OnPhysicalMeleeAttack,
                    Action = AbilityAction.Slash,
                    Effects = new HashSet<Effect> {new PhysicalDamage {Damage = 40}}
                }
            }
        };
    }
}
