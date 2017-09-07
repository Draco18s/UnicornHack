using System.Collections.Generic;
using UnicornHack.Effects;

namespace UnicornHack
{
    public class CalculatedProperty<T> : Property<T>
    {
        private List<ChangedProperty<T>> _sortingList;

        public CalculatedProperty()
        {
        }

        public CalculatedProperty(Entity entity, string name) : base(entity, name)
        {
        }

        public bool IsCurrent { get; set; }

        private T _currentValue;
        public override T CurrentValue
        {
            get
            {
                if (!IsCurrent)
                {
                    UpdateValue();
                }
                return _currentValue;
            }
            set => _currentValue = value;
        }

        public override void UpdateValue()
        {
            if (_sortingList == null)
            {
                _sortingList = new List<ChangedProperty<T>>();
            }
            else
            {
                _sortingList.Clear();
            }

            foreach (var activeEffect in Entity.ActiveEffects)
            {
                if (activeEffect is ChangedProperty<T> nextPropertyChange
                    && nextPropertyChange.PropertyName == Name)
                {
                    _sortingList.Add(nextPropertyChange);
                }
            }

            _sortingList.Sort(ChangedPropertyComparer.Instance);

            var oldValue = LastValue;
            LastValue = ((PropertyDescription<T>)PropertyDescription.Loader.Get(Name)).DefaultValue;
            var state = (0, 0);
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _sortingList.Count; index++)
            {
                _sortingList[index].Apply(this, ref state);
            }

            CurrentValue = LastValue;
            IsCurrent = true;

            Entity.OnPropertyChanged(Name, oldValue, CurrentValue);
        }

        protected class ChangedPropertyComparer : IComparer<ChangedProperty<T>>
        {
            public static readonly ChangedPropertyComparer Instance = new ChangedPropertyComparer();

            private ChangedPropertyComparer()
            {
            }

            public int Compare(ChangedProperty<T> x, ChangedProperty<T> y)
            {
                var xAbility = x.SourceAbility;
                var yAbility = y.SourceAbility;

                if (xAbility != null
                    && yAbility != null)
                {
                    switch (xAbility.Activation)
                    {
                        case AbilityActivation.Always:
                            switch (yAbility.Activation)
                            {
                                case AbilityActivation.Always:
                                    if (xAbility.Name == Actor.InnateAbilityName)
                                    {
                                        if (yAbility.Name != Actor.InnateAbilityName)
                                        {
                                            return -1;
                                        }
                                    }
                                    else if (yAbility.Name == Actor.InnateAbilityName)
                                    {
                                        return 1;
                                    }
                                    break;
                                default:
                                    return -1;
                            }
                            break;
                        case AbilityActivation.WhileToggled:
                            switch (yAbility.Activation)
                            {
                                case AbilityActivation.Always:
                                    return 1;
                                case AbilityActivation.WhileToggled:
                                    break;
                                default:
                                    return -1;
                            }
                            break;
                        case AbilityActivation.WhilePossessed:
                            switch (yAbility.Activation)
                            {
                                case AbilityActivation.Always:
                                case AbilityActivation.WhileToggled:
                                    return 1;
                                case AbilityActivation.WhilePossessed:
                                    break;
                                default:
                                    return -1;
                            }
                            break;
                        case AbilityActivation.WhileEquipped:
                            switch (yAbility.Activation)
                            {
                                case AbilityActivation.Always:
                                case AbilityActivation.WhileToggled:
                                case AbilityActivation.WhilePossessed:
                                    return 1;
                                case AbilityActivation.WhileEquipped:
                                    break;
                                default:
                                    return -1;
                            }
                            break;
                        default:
                            switch (yAbility.Activation)
                            {
                                case AbilityActivation.Always:
                                case AbilityActivation.WhileToggled:
                                case AbilityActivation.WhilePossessed:
                                case AbilityActivation.WhileEquipped:
                                    return 1;
                            }
                            break;
                    }
                }

                var result = y.Function - x.Function;
                if (result != 0)
                {
                    return result;
                }

                return x.Id - y.Id;
            }
        }
    }
}