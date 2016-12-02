using UnicornHack.Models.GameDefinitions;

namespace UnicornHack.Models.GameState.Events
{
    public class ItemEquipmentEvent : SensoryEvent
    {
        public virtual Actor Equipper { get; set; }
        public virtual SenseType EquipperSensed { get; set; }
        public virtual Item Item { get; set; }
        public virtual SenseType ItemSensed { get; set; }

        public static void New(Actor equipper, Item item)
        {
            foreach (var sensor in equipper.Level.Actors)
            {
                var equipperSensed = sensor.CanSense(equipper);
                var itemSensed = sensor.CanSense(item);

                if (equipperSensed == SenseType.None
                    && itemSensed == SenseType.None)
                {
                    continue;
                }

                var @event = new ItemEquipmentEvent
                {
                    Equipper = equipper,
                    EquipperSensed = equipperSensed,
                    Item = item,
                    ItemSensed = itemSensed
                };
                item.AddReference();

                sensor.Sense(@event);
            }
        }

        public override void Delete()
        {
            base.Delete();
            Item?.RemoveReference();
        }
    }
}