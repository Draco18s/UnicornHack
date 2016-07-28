using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using UnicornHack.Models.GameDefinitions;
using UnicornHack.Models.GameState;
using UnicornHack.Models.GameState.Events;

namespace UnicornHack.Services.English
{
    public class EnglishLanguageService : ILanguageService
    {
        protected virtual CultureInfo Culture { get; } = new CultureInfo(name: "en-US");

        private EnglishMorphologicalProcessor EnglishMorphologicalProcessor { get; } =
            new EnglishMorphologicalProcessor();

        #region Game concepts

        private string ToString(Actor sensee, EnglishPerson person, SenseType sense)
            => ToString(
                sensee,
                person,
                sense.HasFlag(SenseType.Sight) || sense.HasFlag(SenseType.Telepathy),
                definiteDeterminer: true);

        private string ToString(Actor actor, EnglishPerson person, bool variantKnown, bool? definiteDeterminer = null)
        {
            if (person == EnglishPerson.Second)
            {
                return EnglishMorphologicalProcessor.GetPronoun(
                    EnglishPronounForm.Normal, EnglishNumber.Singular, person, gender: null);
            }

            if (!variantKnown)
            {
                return "something";
            }

            var monster = actor as Monster;
            if (monster != null)
            {
                var name = monster.Variant.Name +
                           (monster.GivenName == null ? "" : " named \"" + monster.GivenName + "\"");

                var proper = char.IsUpper(name[index: 0]);
                return (definiteDeterminer == null || proper
                    ? ""
                    : definiteDeterminer.Value
                        ? "the "
                        : "a ")
                       + name;
            }

            var character = actor as PlayerCharacter;
            return character.GivenName;
        }

        private string ToString(Item item, SenseType sense)
        {
            if (!sense.HasFlag(SenseType.Sight))
            {
                return "something";
            }

            return ToString(item);
        }

        private string ToString(Item item)
        {
            var dropedItemString = item.Name;
            var stackableItem = item as StackableItem;
            if (stackableItem != null
                && stackableItem.Quantity > 1)
            {
                return stackableItem.Quantity + " " +
                       EnglishMorphologicalProcessor.ProcessNoun(dropedItemString, EnglishNounForm.Plural);
            }

            return "a " + dropedItemString;
        }

        private string ToVerb(AttackType attackType)
        {
            string verb;
            switch (attackType)
            {
                case AttackType.Weapon:
                    // TODO: depends on the weapon type
                    verb = "hit";
                    break;
                case AttackType.Punch:
                    verb = "punch";
                    break;
                case AttackType.Kick:
                    verb = "kick";
                    break;
                case AttackType.Touch:
                    verb = "touch";
                    break;
                case AttackType.Headbutt:
                    verb = "headbutt";
                    break;
                case AttackType.Claw:
                    verb = "claw";
                    break;
                case AttackType.Bite:
                    verb = "bite";
                    break;
                case AttackType.Suck:
                    verb = "suck";
                    break;
                case AttackType.Sting:
                    verb = "sting";
                    break;
                case AttackType.Hug:
                    verb = "squeeze";
                    break;
                case AttackType.Trample:
                    verb = "trample";
                    break;
                case AttackType.Spit:
                    verb = "spit at";
                    break;
                case AttackType.Digestion:
                    verb = "digest";
                    break;
                case AttackType.Spell:
                    verb = "cast a spell at";
                    break;
                case AttackType.Breath:
                    verb = "breath at";
                    break;
                case AttackType.Gaze:
                    verb = "gaze at";
                    break;
                case AttackType.Scream:
                    verb = "scream at";
                    break;
                case AttackType.Explosion:
                    verb = "explode";
                    break;
                case AttackType.OnMeleeHit:
                case AttackType.OnRangedHit:
                case AttackType.OnDeath:
                case AttackType.OnConsumption:
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackType), attackType, message: null);
            }

            return verb;
        }

        private static string ToString(Direction direction)
        {
            switch (direction)
            {
                case Direction.None:
                    return "nowhere";
                case Direction.North:
                    return "north";
                case Direction.South:
                    return "south";
                case Direction.West:
                    return "west";
                case Direction.East:
                    return "east";
                case Direction.Northwest:
                    return "northwest";
                case Direction.Northeast:
                    return "northeast";
                case Direction.Southwest:
                    return "southwest";
                case Direction.Southeast:
                    return "southeast";
                case Direction.Up:
                    return "up";
                case Direction.Down:
                    return "down";
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, message: null);
            }
        }

        #endregion

        #region Events

        public virtual string ToString(AttackEvent @event)
        {
            if (@event.Sensor != @event.Victim
                && @event.Sensor != @event.Attacker
                && !@event.AttackerSensed.HasFlag(SenseType.Sight))
            {
                if (!@event.AttackerSensed.HasFlag(SenseType.Sound)
                    && !@event.AttackerSensed.HasFlag(SenseType.SoundDistant))
                {
                    return null;
                }

                var distanceModifier = @event.AttackerSensed.HasFlag(SenseType.SoundDistant) ? "distant" : null;

                if (@event.AttackType == AttackType.Scream)
                {
                    return ToSentence("You hear a", distanceModifier, "scream.");
                }
                return ToSentence("You hear some", distanceModifier, "noises.");
            }

            var attackerPerson = @event.Sensor == @event.Attacker ? EnglishPerson.Second : EnglishPerson.Third;
            var attacker = ToString(@event.Attacker, attackerPerson, @event.AttackerSensed);

            var victimGender = (EnglishGender)@event.Victim.Sex;
            var victimPerson = @event.Sensor == @event.Victim ? EnglishPerson.Second : EnglishPerson.Third;
            var victim = ToString(@event.Victim, victimPerson, @event.VictimSensed);

            var attackVerb = ToVerb(@event.AttackType);
            var mainVerbForm = attackerPerson == EnglishPerson.Third
                ? EnglishVerbForm.ThirdPersonSingularPresent
                : EnglishVerbForm.BareInfinitive;

            if (@event.Hit)
            {
                var attackSentence = ToSentence(
                    attacker,
                    EnglishMorphologicalProcessor.ProcessVerb(attackVerb, mainVerbForm),
                    @event.Attacker != @event.Victim
                        ? victim
                        : EnglishMorphologicalProcessor.GetPronoun(
                            EnglishPronounForm.Reflexive, EnglishNumber.Singular, attackerPerson, victimGender),
                    @event.Sensor == @event.Victim ? "!" : ".");

                if (!@event.VictimSensed.HasFlag(SenseType.Sight))
                {
                    return attackSentence;
                }

                var damageSentence = "";
                if (victimPerson == EnglishPerson.Second)
                {
                    if (@event.Damage == 0)
                    {
                        damageSentence = "You are unaffected.";
                    }
                    else
                    {
                        damageSentence = Format("[{0} pts.]", @event.Damage);
                    }
                }
                else
                {
                    if (@event.Damage == 0)
                    {
                        damageSentence = ToSentence(victim, "seems unaffected.");
                    }
                    else
                    {
                        damageSentence = Format("({0} pts.)", @event.Damage);
                    }
                }

                return attackSentence + " " + damageSentence;
            }
            return ToSentence(
                attacker,
                EnglishMorphologicalProcessor.ProcessVerb(verbPhrase: "try", form: mainVerbForm),
                EnglishMorphologicalProcessor.ProcessVerb(attackVerb, EnglishVerbForm.Infinitive),
                victim,
                ", but",
                EnglishMorphologicalProcessor.ProcessVerb(verbPhrase: "miss", form: mainVerbForm));
        }

        public virtual string ToString(ItemDropEvent @event)
        {
            var dropperPerson = @event.Sensor == @event.Dropper ? EnglishPerson.Second : EnglishPerson.Third;
            return ToSentence(
                ToString(@event.Dropper, dropperPerson, @event.DropperSensed),
                EnglishMorphologicalProcessor.ProcessVerbSimplePresent(verbPhrase: "drop", person: dropperPerson),
                ToString(@event.Item));
        }

        public virtual string ToString(ItemPickUpEvent @event)
        {
            var pickerPerson = @event.Sensor == @event.Picker ? EnglishPerson.Second : EnglishPerson.Third;
            return ToSentence(
                ToString(@event.Picker, pickerPerson, @event.PickerSensed),
                EnglishMorphologicalProcessor.ProcessVerbSimplePresent(verbPhrase: "pick up", person: pickerPerson),
                ToString(@event.Item));
        }

        public virtual string ToString(DeathEvent @event)
        {
            var deceasedPerson = @event.Sensor == @event.Deceased ? EnglishPerson.Second : EnglishPerson.Third;
            return ToSentence(
                ToString(@event.Deceased, deceasedPerson, @event.DeceasedSensed),
                EnglishMorphologicalProcessor.ProcessVerbSimplePresent(verbPhrase: "die", person: deceasedPerson),
                deceasedPerson == EnglishPerson.Second ? "!" : ".");
        }

        public virtual string ToString(ItemConsumptionEvent @event)
        {
            var consumerPerson = @event.Sensor == @event.Consumer ? EnglishPerson.Second : EnglishPerson.Third;
            return ToSentence(
                ToString(@event.Consumer, consumerPerson, @event.ConsumerSensed),
                EnglishMorphologicalProcessor.ProcessVerbSimplePresent(verbPhrase: "eat", person: consumerPerson),
                ToString(@event.Object, @event.ObjectSensed));
        }

        #endregion

        #region Interface messages

        public virtual string Welcome(PlayerCharacter character)
        {
            return Format("Welcome to the {0}, {1}!", character.Level.Name,
                ToString(character, EnglishPerson.Third, variantKnown: true));
        }

        public virtual string UnableToMove(Direction direction)
        {
            return Format("Can't move {0}.", ToString(direction));
        }

        #endregion

        #region Formatting

        private string Format(string format, params object[] arguments)
        {
            return string.Format(Culture, format, arguments);
        }

        private string ToSentence(params string[] components)
        {
            var builder = new StringBuilder();

            foreach (var component in components)
            {
                if (!string.IsNullOrEmpty(component))
                {
                    builder.Append(component).Append(value: " ");
                }
            }

            var first = builder[index: 0];
            builder.Remove(startIndex: 0, length: 1)
                .Insert(index: 0, value: char.ToUpper(first))
                .Remove(builder.Length - 1, length: 1);

            if (!char.IsPunctuation(builder[builder.Length - 1]))
            {
                builder.Append(value: '.');
            }
            builder.Replace(oldValue: " ,", newValue: ",")
                .Replace(oldValue: " .", newValue: ".")
                .Replace(oldValue: " .", newValue: ".")
                .Replace(oldValue: " !", newValue: "!");

            return builder.ToString();
        }

        #endregion

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }
    }
}