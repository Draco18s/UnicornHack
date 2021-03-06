using System;
using System.Collections.Generic;
using System.IO;
using CSharpScriptSerialization;
using UnicornHack.Generation;
using UnicornHack.Generation.Effects;
using UnicornHack.Generation.Map;
using UnicornHack.Primitives;
using UnicornHack.Utils.DataLoading;

namespace UnicornHack.Editor
{
    public static class Program
    {
        private static readonly bool SerializeToScript = false;

        public static void Main()
        {
            Serialize(Creature.Loader);
            Serialize(PlayerRace.Loader);
            Serialize(Item.Loader);
            Serialize(ItemGroup.Loader);
            Serialize(Ability.Loader);
            Serialize(Branch.Loader);
            Serialize(NormalMapFragment.Loader);
            Serialize(ConnectingMapFragment.Loader);
            Serialize(DefiningMapFragment.Loader);
        }

        private static void Serialize<T>(CSScriptLoaderBase<T> loader, Func<T, T> transform = null)
            where T : class, ILoadable
        {
            Console.WriteLine("Serializing " + typeof(T).Name + " instances...");

            var directory = Path.Combine(AppContext.BaseDirectory, "New", loader.RelativePath);
            Directory.CreateDirectory(directory);
            foreach (var item in loader.GetAll())
            {
                try
                {
                    var itemToSerialize = transform != null ? transform(item) : item;
                    string script = null;
                    if (SerializeToScript)
                    {
                        script = CSScriptSerializer.Serialize(itemToSerialize);
                        File.WriteAllText(
                            Path.Combine(
                                loader.RelativePath,
                                CSScriptLoaderHelpers.GetScriptFilename(itemToSerialize.Name)),
                            script);
                    }
                    else
                    {
                        var code = CSClassSerializer.Serialize(
                            itemToSerialize, itemToSerialize.Name, loader.DataType.Namespace, loader.DataType.Name);
                        File.WriteAllText(
                            Path.Combine(directory, CSScriptLoaderHelpers.GetClassFilename(itemToSerialize.Name)),
                            code);
                    }

                    Verify(script, (dynamic)item);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to serialize " + item.Name, e);
                }
            }
        }

        private static void Verify(string script, PropertyDescription property)
            => Verify(script, property, i => i.Name);

        private static void Verify(string script, Creature creature)
            => Verify(script, creature, c => c.Name, getAbilities: c => c.Abilities);

        private static void Verify(string script, PlayerRace player)
            => Verify(script, player, p => p.Name, getAbilities: c => c.Abilities);

        private static void Verify(string script, Item item)
            => Verify(script, item, i => i.Name, getAbilities: c => c.Abilities);

        private static void Verify(string script, ItemGroup item)
            => Verify(script, item, i => i.Name);

        private static void Verify(string script, Ability item)
            => Verify(script, item, i => i.Name);

        private static void Verify(string script, Branch branch)
            => Verify(script, branch, b => b.Name);

        private static void Verify(string script, MapFragment fragment)
            => Verify(script, fragment, f => f.Name, VerifyNoUnicode);

        private static void Verify(string script, ConnectingMapFragment fragment)
            => Verify(script, fragment, f => f.Name, VerifyNoUnicode);

        private static void Verify(string script, DefiningMapFragment fragment)
            => Verify(script, fragment, f => f.Name, VerifyNoUnicode);

        private static bool VerifyNoUnicode(MapFragment fragment)
        {
            int x = 0, y = 0;
            foreach (var character in fragment.Map)
            {
                switch (character)
                {
                    case '\r':
                        continue;
                    case '\n':
                        x = 0;
                        y++;
                        continue;
                }

                if (character != (byte)character)
                {
                    throw new InvalidOperationException($"Invalid character '{character}' at {x},{y}");
                }

                x++;
            }

            return true;
        }

        private static void Verify<T>(string script, T variant,
            Func<T, string> getName,
            Func<T, bool> isValid = null,
            Func<T, ISet<Ability>> getAbilities = null)
        {
            try
            {
                var serializedVariant = script == null ? variant : CSScriptLoaderHelpers.Load<T>(script);
                var name = getName(variant);
                if (name != getName(serializedVariant)
                    || (isValid != null && !isValid(serializedVariant)))
                {
                    throw new InvalidOperationException("Invalid");
                }

                var abilities = getAbilities?.Invoke(serializedVariant);
                if (abilities != null)
                {
                    foreach (var ability in abilities)
                    {
                        Validate(ability, name);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine(script);
                throw;
            }
        }

        private static void Validate(Ability ability, string ownerName)
        {
            if ((ability.Activation & ActivationType.Slottable) != 0
                && ability.Delay == null && ability.Cooldown == 0 && ability.XPCooldown == 0)
            {
                throw new InvalidOperationException(
                    $"Ability {ability.Name} on {ownerName} has no delay or cooldown");
            }

            if (ability.Effects == null)
            {
                return;
            }

            foreach (var effect in ability.Effects)
            {
                switch (effect)
                {
                    case AddAbility addAbility:
                        if (addAbility.Ability != null)
                        {
                            Validate(addAbility.Ability, ownerName);
                        }
                        else
                        {
                            Ability.Loader.Get(addAbility.AbilityName);
                        }

                        break;
                    case ChangeProperty<int> property:
                        Validate(property);
                        break;
                    case ChangeProperty<bool> property:
                        Validate(property);
                        break;
                    case ChangeProperty<byte> property:
                        Validate(property);
                        break;
                }
            }
        }

        private static void Validate<T>(ChangeProperty<T> property)
            where T : struct, IComparable<T>, IConvertible
        {
            var description = (PropertyDescription<T>)PropertyDescription.Loader.Find(property.PropertyName);
            if (description == null)
            {
                throw new InvalidOperationException("Invalid valued property: " + property.PropertyName);
            }

            if (description.PropertyType != property.Value.GetType()
                || description.PropertyType != typeof(T))
            {
                throw new InvalidOperationException(
                    $"Valued property {property.PropertyName} should be of type {description.PropertyType}");
            }

            if (property.Function != ValueCombinationFunction.Sum
                && property.Function != ValueCombinationFunction.Percent)
            {
                if (description.MinValue?.CompareTo(property.Value) > 0)
                {
                    throw new InvalidOperationException(
                        $"Valued property {property.PropertyName} should be lesser or equal to " +
                        description.MinValue);
                }

                if (description.MaxValue?.CompareTo(property.Value) < 0)
                {
                    throw new InvalidOperationException(
                        $"Valued property {property.PropertyName} should be greater or equal to " +
                        description.MaxValue);
                }
            }
        }
    }
}
