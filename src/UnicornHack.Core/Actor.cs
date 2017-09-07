using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnicornHack.Data.Properties;
using UnicornHack.Effects;
using UnicornHack.Events;
using UnicornHack.Generation;
using UnicornHack.Utils;

namespace UnicornHack
{
    public abstract class Actor : Entity, IItemLocation, IReferenceable
    {
        public const int DefaultActionDelay = 100;
        public const string InnateAbilityName = "innate";
        public const string AttributedAbilityName = "attributed";

        public const string PrimaryMeleeAttackName = "primary melee attack";
        public const string SecondaryMeleeAttackName = "secondary melee attack";
        public const string DoubleMeleeAttackName = "double melee attack";
        public const string AdditionalMeleeAttackName = "additional melee attack";

        public const string PrimaryRangedAttackName = "primary ranged attack";
        public const string SecondaryRangedAttackName = "secondary ranged attack";
        public const string DoubleRangedAttackName = "double ranged attack";
        public const string AdditionalRangedAttackName = "additional ranged attack";

        public virtual Direction Heading { get; set; }
        // TODO: make these properties dynamic
        public Species Species { get; set; }
        public SpeciesClass SpeciesClass { get; set; }
        public Sex Sex { get; set; }
        public int MovementDelay { get; set; }
        public int MaxHP => GetProperty<int>(PropertyData.HitPointMaximum.Name);
        public int HP => GetProperty<int>(PropertyData.HitPoints.Name);
        public bool IsAlive => HP > 0;
        public int MaxEP => GetProperty<int>(PropertyData.EnergyPointMaximum.Name);
        public int EP => GetProperty<int>(PropertyData.EnergyPoints.Name);

        /// <summary>
        ///     Warning: this should only be updated when this actor is acting
        /// </summary>
        public virtual int NextActionTick { get; set; }

        public virtual int Gold { get; set; }
        IEnumerable<Item> IItemLocation.Items => Inventory;
        public virtual ICollection<Item> Inventory { get; } = new HashSet<Item>();

        private static readonly Dictionary<string, List<object>> PropertyListeners = new Dictionary<string, List<object>>();

        static Actor()
        {
            AddPropertyListener<int>(PropertyData.HitPointMaximum.Name, (a, o, n) => a.OnMaxHPChanged(o, n));
            AddPropertyListener<int>(PropertyData.EnergyPointMaximum.Name, (a, o, n) => a.OnMaxEPChanged(o, n));
            AddPropertyListener<int>(PropertyData.Constitution.Name, (a, o, n) => a.OnConstitutionChanged(o, n));
            AddPropertyListener<int>(PropertyData.Willpower.Name, (a, o, n) => a.OnWillpowerChanged(o, n));
            AddPropertyListener<int>(PropertyData.Quickness.Name, (a, o, n) => a.OnQuicknessChanged(o, n));
        }

        protected Actor()
        {
        }

        protected Actor(Level level, byte x, byte y) : base(level.Game)
        {
            LevelX = x;
            LevelY = y;
            Level = level;
            NextActionTick = level.Game.NextPlayerTick;
            level.Actors.Push(this);
            AddReference();

            AddAttributeAbility();
        }

        private int _referenceCount;

        void IReferenceable.AddReference()
        {
            _referenceCount++;
        }

        public TransientReference<Actor> AddReference() => new TransientReference<Actor>(this);

        public void RemoveReference()
        {
            if (_referenceCount > 0
                && --_referenceCount == 0)
            {
                foreach (var item in Inventory.ToList())
                {
                    item.RemoveReference();
                }
                foreach (var ability in Abilities.ToList())
                {
                    Remove(ability);
                }
                Game.Repository.Delete(this);
            }
        }

        private void AddAttributeAbility()
        {
            var ability = new Ability(Game)
            {
                Name = AttributedAbilityName,
                Activation = AbilityActivation.Always,
                Effects = new HashSet<Effect>
                {
                    new ChangeProperty<int>(Game)
                    {
                        PropertyName = PropertyData.HitPointMaximum.Name,
                        Function = ValueCombinationFunction.Sum
                    },
                    new ChangeProperty<int>(Game)
                    {
                        PropertyName = PropertyData.EnergyPointMaximum.Name,
                        Function = ValueCombinationFunction.Sum
                    }
                }
            };

            Add(ability);

            var constitution = GetProperty<int>(PropertyData.Constitution.Name);
            OnConstitutionChanged(constitution, constitution);

            var willpower = GetProperty<int>(PropertyData.Willpower.Name);
            OnWillpowerChanged(willpower, willpower);

            var quickness = GetProperty<int>(PropertyData.Quickness.Name);
            OnQuicknessChanged(quickness, quickness);
        }

        public virtual void RecalculateWeaponAbilities()
        {
            var canUseWeapons = !GetProperty<bool>(PropertyData.Handlessness.Name)
                                && !GetProperty<bool>(PropertyData.Limblessness.Name);

            var additionalMeleeAttack = Abilities.FirstOrDefault(a => a.Name == AdditionalMeleeAttackName);
            if (additionalMeleeAttack == null && canUseWeapons)
            {
                additionalMeleeAttack = new Ability(Game)
                {
                    Name = AdditionalMeleeAttackName,
                    Effects = new HashSet<Effect> { new MeleeAttack(Game), new PhysicalDamage(Game) }
                };

                Add(additionalMeleeAttack);
            }

            var doubleMeleeAttack = Abilities.FirstOrDefault(a => a.Name == DoubleMeleeAttackName);
            if (doubleMeleeAttack == null && canUseWeapons)
            {
                doubleMeleeAttack = new Ability(Game)
                {
                    Name = DoubleMeleeAttackName,
                    Activation = AbilityActivation.OnTarget,
                    // TODO: Calculate proper delay
                    Delay = 100,
                    Effects = new HashSet<Effect>
                    {
                        new MeleeAttack(Game),
                        new PhysicalDamage(Game),
                        new ActivateAbility(Game) {Ability = additionalMeleeAttack.AddReference().Referenced}
                    }
                };

                Add(doubleMeleeAttack);
            }

            var primaryMeleeAttack = Abilities.FirstOrDefault(a => a.Name == PrimaryMeleeAttackName);
            if (primaryMeleeAttack == null && canUseWeapons)
            {
                primaryMeleeAttack = new Ability(Game)
                {
                    Name = PrimaryMeleeAttackName,
                    Activation = AbilityActivation.OnTarget,
                    // TODO: Calculate proper delay
                    Delay = 100,
                    Effects = new HashSet<Effect> {new MeleeAttack(Game), new PhysicalDamage(Game)}
                };

                Add(primaryMeleeAttack);
            }

            var secondaryMeleeAttack = Abilities.FirstOrDefault(a => a.Name == SecondaryMeleeAttackName);
            if (secondaryMeleeAttack == null && canUseWeapons)
            {
                secondaryMeleeAttack = new Ability(Game)
                {
                    Name = SecondaryMeleeAttackName,
                    Activation = AbilityActivation.OnTarget,
                    // TODO: Calculate proper delay
                    Delay = 100,
                    Effects = new HashSet<Effect> {new MeleeAttack(Game), new PhysicalDamage(Game)}
                };

                Add(secondaryMeleeAttack);
            }

            var additionalRangedAttack = Abilities.FirstOrDefault(a => a.Name == AdditionalRangedAttackName);
            if (additionalRangedAttack == null && canUseWeapons)
            {
                additionalRangedAttack = new Ability(Game)
                {
                    Name = AdditionalRangedAttackName,
                    Effects = new HashSet<Effect> { new RangeAttack(Game), new PhysicalDamage(Game) }
                };

                Add(additionalRangedAttack);
            }

            var doubleRangedAttack = Abilities.FirstOrDefault(a => a.Name == DoubleRangedAttackName);
            if (doubleRangedAttack == null && canUseWeapons)
            {
                doubleRangedAttack = new Ability(Game)
                {
                    Name = DoubleRangedAttackName,
                    Activation = AbilityActivation.OnTarget,
                    // TODO: Calculate proper delay
                    Delay = 100,
                    Effects = new HashSet<Effect>
                    {
                        new RangeAttack(Game),
                        new PhysicalDamage(Game),
                        new ActivateAbility(Game) {Ability = additionalRangedAttack.AddReference().Referenced}
                    }
                };

                Add(doubleRangedAttack);
            }

            var primaryRangedAttack = Abilities.FirstOrDefault(a => a.Name == PrimaryRangedAttackName);
            if (primaryRangedAttack == null && canUseWeapons)
            {
                primaryRangedAttack = new Ability(Game)
                {
                    Name = PrimaryRangedAttackName,
                    Activation = AbilityActivation.OnTarget,
                    // TODO: Calculate proper delay
                    Delay = 100,
                    Effects = new HashSet<Effect> {new RangeAttack(Game), new PhysicalDamage(Game)}
                };

                Add(primaryRangedAttack);
            }

            var secondaryRangedAttack = Abilities.FirstOrDefault(a => a.Name == SecondaryRangedAttackName);
            if (secondaryRangedAttack == null && canUseWeapons)
            {
                secondaryRangedAttack = new Ability(Game)
                {
                    Name = SecondaryRangedAttackName,
                    Activation = AbilityActivation.OnTarget,
                    // TODO: Calculate proper delay
                    Delay = 100,
                    Effects = new HashSet<Effect> {new RangeAttack(Game), new PhysicalDamage(Game)}
                };

                Add(secondaryRangedAttack);
            }

            if (primaryMeleeAttack != null)
            {
                primaryMeleeAttack.IsUsable = false;
                primaryMeleeAttack.Effects.OfType<MeleeAttack>().Single().Weapon = null;
            }
            if (secondaryMeleeAttack != null)
            {
                secondaryMeleeAttack.IsUsable = false;
                secondaryMeleeAttack.Effects.OfType<MeleeAttack>().Single().Weapon = null;
            }
            if (doubleMeleeAttack != null)
            {
                doubleMeleeAttack.IsUsable = false;
                doubleMeleeAttack.Effects.OfType<MeleeAttack>().Single().Weapon = null;
            }
            if (additionalMeleeAttack != null)
            {
                additionalMeleeAttack.IsUsable = false;
                additionalMeleeAttack.Effects.OfType<MeleeAttack>().Single().Weapon = null;
            }
            if (primaryRangedAttack != null)
            {
                primaryRangedAttack.IsUsable = false;
                primaryRangedAttack.Effects.OfType<RangeAttack>().Single().Weapon = null;
            }
            if (secondaryRangedAttack != null)
            {
                secondaryRangedAttack.IsUsable = false;
                secondaryRangedAttack.Effects.OfType<RangeAttack>().Single().Weapon = null;
            }
            if (doubleRangedAttack != null)
            {
                doubleRangedAttack.IsUsable = false;
                doubleRangedAttack.Effects.OfType<RangeAttack>().Single().Weapon = null;
            }
            if (additionalRangedAttack != null)
            {
                additionalRangedAttack.IsUsable = false;
                additionalRangedAttack.Effects.OfType<RangeAttack>().Single().Weapon = null;
            }

            if (!canUseWeapons)
            {
                return;
            }

            Item twoHandedWeapon = null;
            Item primaryWeapon = null;
            Item secondaryWeapon = null;
            foreach (var item in Inventory)
            {
                if (item.EquippedSlot.HasValue)
                {
                    switch (item.EquippedSlot)
                    {
                        case EquipmentSlot.GraspBothExtremities:
                            twoHandedWeapon = item;
                            break;
                        case EquipmentSlot.GraspPrimaryExtremity:
                            primaryWeapon = item;
                            break;
                        case EquipmentSlot.GraspSecondaryExtremity:
                            secondaryWeapon = item;
                            break;
                    }
                }
            }

            var primaryWeaponType = primaryWeapon?.Type ?? ItemType.WeaponMeleeFist;
            var secondaryWeaponType = secondaryWeapon?.Type ?? ItemType.WeaponMeleeFist;
            var dualWielding = primaryWeapon != null && secondaryWeapon != null;
            var dualFist = primaryWeaponType.HasFlag(ItemType.WeaponMeleeFist)
                           && secondaryWeaponType.HasFlag(ItemType.WeaponMeleeFist);

            if (twoHandedWeapon != null)
            {
                primaryMeleeAttack.IsUsable = true;
                primaryMeleeAttack.Effects.OfType<MeleeAttack>().Single().Weapon = twoHandedWeapon;

                primaryRangedAttack.IsUsable = true;
                primaryRangedAttack.Effects.OfType<RangeAttack>().Single().Weapon = twoHandedWeapon;
            }
            else
            {
                if ((dualWielding
                     && !primaryWeaponType.HasFlag(ItemType.WeaponMeleeFist)
                     && !secondaryWeaponType.HasFlag(ItemType.WeaponMeleeFist))
                    || dualFist)
                {
                    doubleMeleeAttack.IsUsable = true;
                    doubleMeleeAttack.Effects.OfType<MeleeAttack>().Single().Weapon = primaryWeapon;

                    additionalMeleeAttack.IsUsable = true;
                    additionalMeleeAttack.Effects.OfType<MeleeAttack>().Single().Weapon = secondaryWeapon;

                    if (!dualFist)
                    {
                        doubleRangedAttack.IsUsable = true;
                        doubleRangedAttack.Effects.OfType<RangeAttack>().Single().Weapon = primaryWeapon;

                        additionalRangedAttack.IsUsable = true;
                        additionalRangedAttack.Effects.OfType<RangeAttack>().Single().Weapon = secondaryWeapon;
                    }
                }

                primaryMeleeAttack.IsUsable = true;
                primaryMeleeAttack.Effects.OfType<MeleeAttack>().Single().Weapon = primaryWeapon;

                secondaryMeleeAttack.IsUsable = true;
                secondaryMeleeAttack.Effects.OfType<MeleeAttack>().Single().Weapon = secondaryWeapon;

                if (!primaryWeaponType.HasFlag(ItemType.WeaponMeleeFist))
                {
                    primaryRangedAttack.IsUsable = true;
                    primaryRangedAttack.Effects.OfType<RangeAttack>().Single().Weapon = primaryWeapon;
                }
                if (!secondaryWeaponType.HasFlag(ItemType.WeaponMeleeFist))
                {
                    secondaryRangedAttack.IsUsable = true;
                    secondaryRangedAttack.Effects.OfType<RangeAttack>().Single().Weapon = secondaryWeapon;
                }
            }
        }

        /// <returns>Returns <c>false</c> if the actor hasn't finished their turn.</returns>
        public abstract bool Act();

        public virtual void Sense(SensoryEvent @event)
        {
            @event.Game = Game;
        }

        public virtual SenseType CanSense(Entity target)
        {
            var sense = SenseType.None;
            if (target == this) // Or is adjecent
            {
                sense |= SenseType.Touch;
            }

            sense |= SenseType.Sight;

            return sense;
        }

        public virtual bool UseStairs(bool up, bool pretend = false)
        {
            var stairs = Level.Connections.SingleOrDefault(s => s.LevelX == LevelX && s.LevelY == LevelY);

            var moveToLevel = stairs?.TargetLevel;
            if (moveToLevel == null)
            {
                return false;
            }

            if (pretend)
            {
                return true;
            }

            NextActionTick += MovementDelay;
            var moveToLevelX = stairs.TargetLevelX;
            var moveToLevelY = stairs.TargetLevelY;

            if (!moveToLevel.Players.Any())
            {
                var previousNextPlayerTick = Game.NextPlayerTick;
                Game.NextPlayerTick = NextActionTick;

                // Catch up the level to current turn
                // TODO: Instead of this put actor in 'traveling' state till its next action
                var waitedFor = moveToLevel.Turn();
                Debug.Assert(waitedFor == null);

                Game.NextPlayerTick = previousNextPlayerTick;
            }

            var conflictingActor =
                moveToLevel.Actors.SingleOrDefault(a => a.LevelX == moveToLevelX && a.LevelY == moveToLevelY);
            conflictingActor?.GetDisplaced();

            if (moveToLevel.BranchName == "surface")
            {
                ChangeCurrentHP(-1 * HP);
                return true;
            }

            using (AddReference())
            {
                Level.Actors.Remove(this);
                Level = moveToLevel;
                LevelX = moveToLevelX.Value;
                LevelY = moveToLevelY.Value;
                moveToLevel.Actors.Push(this);

                ActorMoveEvent.New(this, movee: null, eventOrder: Game.EventOrder++);
            }

            return true;
        }

        public virtual bool Move(Direction direction, bool pretend = false)
        {
            if (Heading != direction)
            {
                if (pretend)
                {
                    return true;
                }

                var octants = direction.ClosestOctantsTo(Heading);

                NextActionTick += (MovementDelay * octants) / 4;

                Heading = direction;

                // TODO: Fire event

                return true;
            }

            var targetCell = ToLevelCell(Vector.Convert(direction));
            if (!targetCell.HasValue)
            {
                return false;
            }

            var conflictingActor =
                Level.Actors.SingleOrDefault(a => a.LevelX == targetCell.Value.X && a.LevelY == targetCell.Value.Y);
            if (conflictingActor != null)
            {
                var handled = HandleBlockingActor(conflictingActor, pretend);
                if (handled.HasValue)
                {
                    return handled.Value;
                }
            }

            if (MovementDelay == 0)
            {
                return false;
            }

            if (!Reposition(targetCell.Value, pretend))
            {
                return false;
            }

            if (pretend)
            {
                return true;
            }

            ActorMoveEvent.New(this, movee: null, eventOrder: Game.EventOrder++);

            // TODO: take terrain into account
            NextActionTick += MovementDelay;

            return true;
        }

        protected virtual bool? HandleBlockingActor(Actor actor, bool pretend)
            => false;

        private bool Reposition(Point targetCell, bool pretend)
        {
            if (!Level.CanMoveTo(targetCell))
            {
                return false;
            }

            if (pretend)
            {
                return true;
            }

            LevelX = targetCell.X;
            LevelY = targetCell.Y;
            var itemsOnNewCell = Level.Items.Where(i => i.LevelX == targetCell.X && i.LevelY == targetCell.Y).ToList();
            foreach (var itemOnNewCell in itemsOnNewCell)
            {
                PickUp(itemOnNewCell);
            }

            return true;
        }

        public virtual bool GetDisplaced()
        {
            // TODO: displace other actors
            var possibleDirectionsToMove = Level.GetPossibleMovementDirections(new Point(LevelX, LevelY), safe: true);
            if (possibleDirectionsToMove.Count == 0)
            {
                NextActionTick += DefaultActionDelay;
                return true;
            }
            var directionIndex = Game.Random.Next(minValue: 0, maxValue: possibleDirectionsToMove.Count);

            var targetCell = ToLevelCell(Vector.Convert(possibleDirectionsToMove[directionIndex]));
            if (targetCell != null)
            {
                return Reposition(targetCell.Value, pretend: false);
            }

            // TODO: fire event

            return false;
        }

        public virtual bool Equip(Item item, EquipmentSlot slot, bool pretend = false)
        {
            var equipped = GetEquippedItem(slot);
            if (equipped == item)
            {
                return true;
            }

            if (!item.EquipableSlots.HasFlag(slot))
            {
                return false;
            }

            if (pretend)
            {
                return true;
            }

            // TODO: Calculate delay
            NextActionTick += DefaultActionDelay;

            if (equipped == null)
            {
                if (slot == EquipmentSlot.GraspPrimaryExtremity || slot == EquipmentSlot.GraspSecondaryExtremity)
                {
                    Unequip(GetEquippedItem(EquipmentSlot.GraspBothExtremities));
                }
                else if (slot == EquipmentSlot.GraspBothExtremities)
                {
                    Unequip(GetEquippedItem(EquipmentSlot.GraspPrimaryExtremity));
                    Unequip(GetEquippedItem(EquipmentSlot.GraspSecondaryExtremity));
                }
            }
            else
            {
                Unequip(equipped);
            }

            item.EquippedSlot = slot;
            ItemEquipmentEvent.New(this, item, Game.EventOrder++);

            foreach (var ability in item.Abilities.Where(
                a => a.Activation == AbilityActivation.WhileEquipped && !a.IsActive))
            {
                ability.Activate(new AbilityActivationContext
                {
                    Activator = this,
                    Target = this,
                    AbilityTrigger = AbilityActivation.WhileEquipped
                });
            }

            RecalculateWeaponAbilities();

            return true;
        }

        public virtual bool Unequip(Item item, bool pretend = false)
        {
            if (item?.EquippedSlot == null)
            {
                return false;
            }

            if (pretend)
            {
                return true;
            }

            // TODO: Calculate delay
            NextActionTick += DefaultActionDelay;

            item.EquippedSlot = null;

            // TODO: Suppress message when thrown
            ItemUnequipmentEvent.New(this, item, Game.EventOrder++);

            foreach (var ability in item.Abilities.Where(a
                => a.Activation == AbilityActivation.WhileEquipped && a.IsActive))
            {
                ability.Deactivate();
            }

            RecalculateWeaponAbilities();

            return true;
        }

        public virtual bool Quaff(Item item, bool pretend = false)
        {
            if (item.Type != ItemType.Potion)
            {
                return false;
            }

            if (pretend)
            {
                return true;
            }

            // TODO: Calculate delay
            NextActionTick += DefaultActionDelay;

            using (var reference = item.Split(1))
            {
                var splitItem = reference.Referenced;

                ItemConsumptionEvent.New(this, splitItem, Game.EventOrder++);

                foreach (var ability in splitItem.Abilities.Where(a => a.Activation == AbilityActivation.OnConsumption))
                {
                    ability.Activate(new AbilityActivationContext
                    {
                        Activator = this,
                        Target = this,
                        AbilityTrigger = AbilityActivation.OnDigestion
                    });
                }
            }

            return true;
        }

        public virtual bool PickUp(Item item, bool pretend = false)
        {
            if (pretend)
            {
                return true;
            }

            // TODO: Calculate delay
            NextActionTick += DefaultActionDelay;

            using (item.AddReference())
            {
                item.MoveTo(this);
                ItemPickUpEvent.New(this, item, Game.EventOrder++);
            }

            return true;
        }

        public virtual bool DropGold(int quantity, bool pretend = false)
        {
            if (quantity == 0 || quantity > Gold)
            {
                return false;
            }

            if (pretend)
            {
                return true;
            }

            NextActionTick += DefaultActionDelay;

            Gold -= quantity;
            var item = GoldVariant.Get().Instantiate(new LevelCell(Level, LevelX, LevelY), quantity).Single();

            ItemDropEvent.New(this, item, Game.EventOrder++);

            return true;
        }

        public virtual bool Drop(Item item, bool pretend = false)
        {
            if (item.EquippedSlot != null && !Unequip(item, pretend))
            {
                return false;
            }

            if (pretend)
            {
                return true;
            }

            // TODO: Calculate AP cost
            NextActionTick += DefaultActionDelay;

            using (item.AddReference())
            {
                item.MoveTo(new LevelCell(Level, LevelX, LevelY));

                ItemDropEvent.New(this, item, Game.EventOrder++);
            }

            return true;
        }

        public virtual bool TryAdd(IEnumerable<Item> items)
        {
            var succeeded = true;
            foreach (var item in items.ToList())
            {
                succeeded = TryAdd(item) && succeeded;
            }

            return succeeded;
        }

        public virtual bool TryAdd(Item item)
        {
            if (!CanAdd(item))
            {
                return false;
            }

            var itemOrStack = item.StackWith(Inventory);
            if (itemOrStack != null)
            {
                itemOrStack.ActorId = Id;
                itemOrStack.Actor = this;
                Inventory.Add(itemOrStack);
                itemOrStack.AddReference();
            }

            foreach (var ability in item.Abilities.Where(a
                => a.Activation == AbilityActivation.WhilePossessed && !a.IsActive))
            {
                ability.Activate(new AbilityActivationContext
                {
                    Activator = this,
                    Target = this,
                    AbilityTrigger = AbilityActivation.WhilePossessed
                });
            }

            return true;
        }

        public virtual bool CanAdd(Item item) => true;

        public virtual bool Remove(Item item)
        {
            if (item.EquippedSlot != null)
            {
                Unequip(item);
            }

            foreach (var ability in item.Abilities.Where(a
                => a.Activation == AbilityActivation.WhilePossessed && a.IsActive))
            {
                ability.Deactivate();
            }

            item.ActorId = null;
            item.Actor = null;
            if (Inventory.Remove(item))
            {
                item.RemoveReference();
                return true;
            }
            return false;
        }

        public override void OnPropertyChanged<T>(string propertyName, T oldValue, T newValue)
        {
            if (PropertyListeners.TryGetValue(propertyName, out var listeners))
            {
                foreach (var listener in listeners)
                {
                    ((Action<Actor, T, T>)listener)(this, oldValue, newValue);
                }
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        public override bool HasListener(string propertyName)
            => PropertyListeners.ContainsKey(propertyName)
               || base.HasListener(propertyName);

        private static void AddPropertyListener<T>(string propertyName, Action<Actor, T, T> action)
        {
            if (!PropertyListeners.TryGetValue(propertyName, out var listeners))
            {
                listeners = new List<object>();
                PropertyListeners[propertyName] = listeners;
            }

            listeners.Add(action);
        }

        private void OnMaxHPChanged(int oldValue, int newValue)
        {
            var newHP = HP * newValue / oldValue;
            ChangeCurrentHP(newHP - HP);
        }

        private void OnMaxEPChanged(int oldValue, int newValue)
        {
            var newEP = EP * newValue / oldValue;
            ChangeCurrentEP(newEP - EP);
        }

        private void OnConstitutionChanged(int oldValue, int newValue)
        {
            var effect = Abilities.First(a => a.Name == AttributedAbilityName).ActiveEffects
                .OfType<ChangedProperty<int>>().First(e => e.PropertyName == PropertyData.HitPointMaximum.Name);
            effect.Value = newValue * 10;
            effect.UpdateProperty();
        }

        private void OnWillpowerChanged(int oldValue, int newValue)
        {
            var effect = Abilities.First(a => a.Name == AttributedAbilityName).ActiveEffects
                .OfType<ChangedProperty<int>>().First(e => e.PropertyName == PropertyData.EnergyPointMaximum.Name);
            effect.Value = newValue * 10;
            effect.UpdateProperty();
        }

        private void OnQuicknessChanged(int oldValue, int newValue)
        {
            MovementDelay = DefaultActionDelay * 10 / GetProperty<int>(PropertyData.Quickness.Name);
        }

        public virtual bool ChangeCurrentHP(int hp)
        {
            if (!IsAlive)
            {
                return false;
            }

            var hpProperty = InvalidateProperty<int>(PropertyData.HitPoints.Name);
            var newHP = hpProperty.LastValue + hp;

            if (newHP > MaxHP)
            {
                newHP = MaxHP;
            }

            hpProperty.CurrentValue = newHP;

            if (!IsAlive)
            {
                Die();
                return false;
            }

            return true;
        }

        public virtual void ChangeCurrentEP(int ep)
        {
            var epProperty = InvalidateProperty<int>(PropertyData.EnergyPoints.Name);
            var newEP = epProperty.LastValue + ep;

            if (newEP < 0)
            {
                newEP = 0;
            }
            if (EP > MaxEP)
            {
                newEP = MaxEP;
            }

            epProperty.CurrentValue = newEP;
        }

        protected virtual void Die()
        {
            DeathEvent.New(this, corpse: null, eventOrder: Game.EventOrder++);
        }

        public virtual Item GetEquippedItem(EquipmentSlot slot) =>
            Inventory.FirstOrDefault(item => item.EquippedSlot == slot);

        public virtual Point? ToLevelCell(Vector direction)
        {
            var newX = LevelX + direction.X;
            var newY = LevelY + direction.Y;
            if (newX < 0)
            {
                return null;
            }

            if (newX >= Level.Width)
            {
                return null;
            }

            if (newY < 0)
            {
                return null;
            }

            if (newY >= Level.Height)
            {
                return null;
            }

            return new Point((byte)newX, (byte)newY);
        }

        public class TickComparer : IComparer<Actor>
        {
            public static readonly TickComparer Instance = new TickComparer();

            private TickComparer()
            {
            }

            public int Compare(Actor x, Actor y) => x.NextActionTick - y.NextActionTick;
        }
    }
}