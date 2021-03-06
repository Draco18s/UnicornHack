using System.Collections.Generic;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Primitives;

namespace UnicornHack.Data.Items
{
    public static partial class ItemData
    {
        public static readonly Item CloakOfInvisibility = new Item
        {
            Name = "cloak of invisibility",
            Abilities = new HashSet<Ability>
            {
                new Ability
                {
                    Activation = ActivationType.WhileEquipped,
                    EnergyPointCost = 50,
                    Effects = new HashSet<Effect>
                    {
                        new ChangeProperty<int>
                            {PropertyName = "Visibility", Function = ValueCombinationFunction.Override}
                    }
                }
            },
            Type = ItemType.AccessoryBack,
            Material = Material.Cloth,
            Weight = 5,
            EquipableSizes = SizeCategory.Small | SizeCategory.Medium | SizeCategory.Large,
            EquipableSlots = EquipmentSlot.Back
        };
    }
}
