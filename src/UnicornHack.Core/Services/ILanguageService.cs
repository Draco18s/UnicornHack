using UnicornHack.Events;

namespace UnicornHack.Services
{
    public interface ILanguageService
    {
        string ToString(AttackEvent @event);
        string ToString(DeathEvent @event);
        string ToString(ItemConsumptionEvent @event);
        string ToString(ItemPickUpEvent @event);
        string ToString(ItemDropEvent @event);
        string ToString(ItemEquipmentEvent @event);
        string ToString(ItemUnequipmentEvent @event);
        string ToString(Item item);
        string ToString(Property property);
        string ToString(Ability ability);
        string ToString(EquipmentSlot slot, Actor actor, bool abbreviate);
        string Welcome(Player character);
        string InvalidTarget();
        string UnableToMove(Direction direction);
    }
}