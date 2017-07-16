using System;
using System.Collections.Generic;
using System.Diagnostics;
using CSharpScriptSerialization;
using UnicornHack.Generation;
using UnicornHack.Utils;

namespace UnicornHack
{
    public class Item : IReferenceable, ICSScriptSerializable, ILoadable
    {
        #region State

        private ItemType? _itemType;
        private int? _weight;
        private Material? _material;
        private Size? _equipableSizes;
        private bool? _nameable;
        private int? _stackSize;
        private ISet<string> _simpleProperties;
        private IDictionary<string, object> _valuedProperties;
        private EquipmentSlot? _equipableSlots;

        public virtual int Id { get; private set; }
        public virtual string Name { get; set; }

        public virtual string BaseName { get; set; }
        public Item BaseItem => BaseName == null ? null : Loader.Get(BaseName);

        public virtual ItemType Type
        {
            get { return _itemType ?? BaseItem?.Type ?? ItemType.None; }
            set { _itemType = value; }
        }

        public virtual Weight GenerationWeight { get; set; }

        /// <summary> 100g units </summary>
        public virtual int Weight
        {
            get { return _weight ?? BaseItem?.Weight ?? 0; }
            set { _weight = value; }
        }

        public virtual Material Material
        {
            get { return _material ?? BaseItem?.Material ?? Material.Default; }
            set { _material = value; }
        }

        public virtual bool Nameable
        {
            get { return _nameable ?? BaseItem?.Nameable ?? true; }
            set { _nameable = value; }
        }

        public virtual int StackSize
        {
            get { return _stackSize ?? BaseItem?.StackSize ?? 1; }
            set { _stackSize = value; }
        }

        public virtual Size EquipableSizes
        {
            get { return _equipableSizes ?? BaseItem?.EquipableSizes ?? Size.All; }
            set { _equipableSizes = value; }
        }

        public virtual EquipmentSlot EquipableSlots
        {
            get { return _equipableSlots ?? BaseItem?.EquipableSlots ?? EquipmentSlot.Default; }
            set { _equipableSlots = value; }
        }

        public virtual ISet<Ability> Abilities { get; set; } = new HashSet<Ability>();

        public virtual ISet<string> SimpleProperties
        {
            get
            {
                if (_simpleProperties != null)
                {
                    return _simpleProperties;
                }
                if (BaseItem != null)
                {
                    return BaseItem.SimpleProperties;
                }
                return _simpleProperties = new HashSet<string>();
            }
            set { _simpleProperties = value; }
        }

        public virtual IDictionary<string, object> ValuedProperties
        {
            get
            {
                if (_valuedProperties != null)
                {
                    return _valuedProperties;
                }
                if (BaseItem != null)
                {
                    return BaseItem.ValuedProperties;
                }
                return _valuedProperties = new Dictionary<string, object>();
            }
            set { _valuedProperties = value; }
        }

        public int? ActorId { get; set; }
        public Actor Actor { get; set; }
        public EquipmentSlot? EquippedSlot { get; set; }
        public int? ContainerId { get; set; }
        public Container Container { get; set; }
        public string BranchName { get; set; }
        public byte? LevelDepth { get; set; }
        public Level Level { get; set; }
        public byte? LevelX { get; set; }
        public byte? LevelY { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int GameId { get; private set; }

        public Game Game { get; set; }

        public EquipmentSlot GetEquipableSlots(Size size)
        {
            if (EquipableSizes.HasFlag(size))
            {
                return EquipableSlots;
            }

            var slots = EquipmentSlot.Default;
            if (EquipableSlots.HasFlag(EquipmentSlot.GraspBothExtremities)
                && EquipableSlots.HasFlag(EquipmentSlot.GraspSingleExtremity))
            {
                if (size != Size.Tiny)
                {
                    var smallerSize = (Size)((int)size >> 1);
                    if (EquipableSizes.HasFlag(smallerSize))
                    {
                        slots |= EquipmentSlot.GraspSingleExtremity;
                    }
                }

                if (size != Size.Gigantic)
                {
                    var biggerSize = (Size)((int)size << 1);
                    if (EquipableSizes.HasFlag(biggerSize))
                    {
                        slots |= EquipmentSlot.GraspBothExtremities;
                    }
                }
            }

            return slots;
        }

        #endregion

        #region Creation

        public Item()
        {
        }

        public Item(Game game)
            : this()
        {
            Game = game;
            Id = game.NextItemId++;
            game.Items.Add(this);
        }

        public virtual Item Instantiate(Game game)
        {
            if (Game != null)
            {
                throw new InvalidOperationException("This item is already part of a game.");
            }

            var itemInstance = CreateInstance(game);
            itemInstance.BaseName = Name;
            itemInstance.Type = Type;
            itemInstance.Weight = Weight;
            itemInstance.Material = Material;
            itemInstance.Nameable = Nameable;
            itemInstance.StackSize = StackSize;
            itemInstance.EquipableSizes = EquipableSizes;

            foreach (var ability in Abilities)
            {
                itemInstance.Abilities.Add(ability.Instantiate(game).AddReference().Referenced);
            }

            return itemInstance;
        }

        public virtual IReadOnlyList<Item> Instantiate(IItemLocation location, int? quantity = null)
        {
            var items = new List<Item>();
            quantity = quantity ?? 1;
            for (var i = 0; i < quantity; i++)
            {
                var item = Instantiate(location.Game);
                items.Add(item);
                if (!item.MoveTo(location))
                {
                    foreach (var createdItem in items)
                    {
                        createdItem.Remove();
                    }
                    return new List<Item>();
                }
            }

            return items;
        }

        protected virtual Item CreateInstance(Game game) => new Item(game);

        private int _referenceCount;

        void IReferenceable.AddReference()
            => _referenceCount++;

        public TransientReference<Item> AddReference()
            => new TransientReference<Item>(this);

        public void RemoveReference()
        {
            if (--_referenceCount <= 0)
            {
                foreach (var ability in Abilities)
                {
                    ability.ItemId = null;
                    ability.RemoveReference();
                }
                // TODO: uncomment when bug fixed
                //Game.Repository.Delete(this);
            }
        }

        #endregion

        #region Actions

        public virtual bool MoveTo(IItemLocation location)
        {
            if (!location.CanAdd(this))
            {
                return false;
            }

            using (AddReference())
            {
                Remove();
                location.TryAdd(this);
            }

            return true;
        }

        protected virtual void ReplaceWith(Item item)
        {
            if (Container != null)
            {
                var container = Container;
                Container.Remove(this);
                var added = container.TryAdd(item);
                Debug.Assert(added);
            }

            if (Actor != null)
            {
                var actor = Actor;
                Actor.Remove(this);
                var added = actor.TryAdd(item);
                Debug.Assert(added);
            }

            if (Level != null)
            {
                var level = Level;
                var levelX = LevelX;
                var levelY = LevelY;
                Level.Remove(this);
                var added = level.TryAdd(item, levelX.Value, levelY.Value);
                Debug.Assert(added);
            }

            Debug.Assert(_referenceCount == 0);
        }

        public virtual Item StackWith(IEnumerable<Item> existingItems)
        {
            if (StackSize <= 1)
            {
                return this;
            }

            foreach (var existingItem in existingItems)
            {
                if (existingItem.BaseName == BaseName)
                {
                    var stack = existingItem as ItemStack;
                    if (stack == null)
                    {
                        stack = new ItemStack(BaseItem, Game);

                        existingItem.MoveTo(stack);

                        stack.TryAdd(this);
                        return stack;
                    }

                    if (!stack.CanAdd(this))
                    {
                        continue;
                    }

                    stack.TryAdd(this);
                    return null;
                }
            }

            return this;
        }

        protected void Remove()
        {
            Actor?.Remove(this);

            Container?.Remove(this);

            Level?.Remove(this);
        }

        public virtual TransientReference<Item> Split(int quantity)
        {
            if (quantity != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity));
            }

            var result = new TransientReference<Item>(this);

            Remove();

            return result;
        }

        private Func<string, byte, int, float> _weightFunction;

        public virtual float GetWeight(Level level)
        {
            if (_weightFunction == null)
            {
                _weightFunction = (GenerationWeight ?? new DefaultWeight()).CreateItemWeightFunction();
            }

            return _weightFunction(level.Branch.Name, level.Depth, 0);
        }

        #endregion

        #region Serialization

        public void OnLoad()
        {
        }

        public static readonly SingleCSScriptLoader<ItemGroup> ItemGroupLoader =
            new SingleCSScriptLoader<ItemGroup>(@"data\item_groups.csx");

        public static readonly GroupedCSScriptLoader<ItemGroup, Item> Loader =
            new GroupedCSScriptLoader<ItemGroup, Item>(@"data\items\", i => ItemGroupLoader.Object.GetGroups(i));

        private static readonly CSScriptSerializer Serializer =
            new PropertyCSScriptSerializer<Item>(GetPropertyConditions<Item>());

        protected static Dictionary<string, Func<TItem, object, bool>> GetPropertyConditions<TItem>()
            where TItem : Item
        {
            return new Dictionary<string, Func<TItem, object, bool>>
            {
                {nameof(Name), (o, v) => v != null},
                {nameof(BaseName), (o, v) => v != null},
                {nameof(Type), (o, v) => (ItemType)v != (o.BaseItem?.Type ?? ItemType.None)},
                {nameof(Weight), (o, v) => (int)v != (o.BaseItem?.Weight ?? 0)},
                {nameof(GenerationWeight), (o, v) => (Weight)v != o.BaseItem?.GenerationWeight},
                {nameof(Material), (o, v) => (Material)v != (o.BaseItem?.Material ?? Material.Default)},
                {nameof(Nameable), (o, v) => (bool)v != (o.BaseItem?.Nameable ?? true)},
                {nameof(StackSize), (o, v) => (int)v != (o.BaseItem?.StackSize ?? 1)},
                {nameof(EquipableSizes), (o, v) => (Size)v != (o.BaseItem?.EquipableSizes ?? Size.All)},
                {
                    nameof(EquipableSlots),
                    (o, v) => (EquipmentSlot)v != (o.BaseItem?.EquipableSlots ?? EquipmentSlot.Default)
                },
                {nameof(Abilities), (o, v) => ((ICollection<Ability>)v).Count != 0},
                {nameof(SimpleProperties), (o, v) => ((ICollection<string>)v).Count != 0},
                {nameof(ValuedProperties), (o, v) => ((IDictionary<string, object>)v).Keys.Count != 0}
            };
        }

        public virtual ICSScriptSerializer GetSerializer() => Serializer;

        #endregion
    }
}