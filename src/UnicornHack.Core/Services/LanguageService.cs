using System;
using System.ComponentModel;
using System.Globalization;
using UnicornHack.Models.GameDefinitions;
using UnicornHack.Models.GameState;
using UnicornHack.Models.GameState.Events;

namespace UnicornHack.Services
{
    public class LanguageService
    {
        protected virtual CultureInfo Culture { get; } = new CultureInfo(name: "en-US");
        private EnglishPluralizationService EnglishPluralizationService { get; } = new EnglishPluralizationService();

        public virtual string Format(string format, params object[] arguments)
        {
            return string.Format(Culture, format, arguments);
        }

        #region Natural language helpers

        static string ToUppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[index: 0]) + s.Substring(startIndex: 1);
        }

        private string ToReflexivePronoun(Sex sex, bool thirdPerson)
        {
            if (!thirdPerson)
            {
                return "yourself";
            }

            switch (sex)
            {
                case Sex.Male:
                    return "himself";
                case Sex.Female:
                    return "herself";
                default:
                    return "itself";
            }
        }

        #endregion

        #region Game concepts

        private string ToString(Actor actor, bool? definiteDeterminer = null)
        {
            var monster = actor as Monster;
            if (monster != null)
            {
                var name = monster.Variant.Name +
                           (monster.GivenName == null ? "" : " named \"" + monster.GivenName + "\"");

                var proper = char.IsUpper(name[0]);
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

        private string ToString(Item item)
        {
            var dropedItemString = item.Name;
            var stackableItem = item as StackableItem;
            if (stackableItem != null)
            {
                dropedItemString = stackableItem.Quantity + " " +
                                   (stackableItem.Quantity > 1
                                       ? EnglishPluralizationService.Pluralize(dropedItemString)
                                       : dropedItemString);
            }

            return dropedItemString;
        }

        private string ToVerb(AttackType attackType, EnglishVerbForm form, bool singularThirdPerson)
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
                    verb = "cast at";
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

            switch (form)
            {
                case EnglishVerbForm.Infinitive:
                    return "to " + verb;
                case EnglishVerbForm.BareInfinitive:
                    return singularThirdPerson ? EnglishPluralizationService.GetSForm(verb) : verb;
                default:
                    throw new ArgumentOutOfRangeException(nameof(form), form, message: null);
            }
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
                && @event.Attacker != @event.Victim
                && !@event.AttackerSensed.HasFlag(SenseType.Sight))
            {
                var noisesFormat = "You hear some {0}noises.";
                if (@event.AttackerSensed.HasFlag(SenseType.Sound))
                {
                    return Format(noisesFormat, "");
                }
                else if (@event.AttackerSensed.HasFlag(SenseType.SoundDistant))
                {
                    return Format(noisesFormat, "distant ");
                }

                return null;
            }

            var attacker = @event.Sensor == @event.Attacker
                ? "you"
                : @event.AttackerSensed.HasFlag(SenseType.Sight)
                    ? ToString(@event.Attacker, definiteDeterminer: true)
                    : "something";

            var victim = @event.Attacker == @event.Victim
                    ? ToReflexivePronoun(@event.Victim.Sex, @event.Sensor != @event.Victim)
                    : @event.Sensor == @event.Victim
                        ? "you"
                        : @event.VictimSensed.HasFlag(SenseType.Sight)
                            ? ToString(@event.Victim, definiteDeterminer: true)
                            : "something";

            if (@event.Hit)
            {
                var attackSentence = ToUppercaseFirst(attacker) + " " +
                                     ToVerb(@event.AttackType, EnglishVerbForm.ThirdPersonSingularPresent, @event.Sensor != @event.Attacker);

                if (@event.Sensor != @event.Victim)
                {
                    attackSentence += " " + victim;
                }

                attackSentence += @event.Sensor == @event.Victim ? "!" : ".";

                var damageSentence = "";
                if (!@event.VictimSensed.HasFlag(SenseType.Sight))
                {
                    return attackSentence;
                }
                if (@event.Sensor == @event.Victim)
                {
                    if (@event.Damage == 0)
                    {
                        damageSentence = "You are unaffected.";
                    }
                    else
                    {
                        damageSentence = Format(" [{0} pts.]", @event.Damage);
                    }
                }
                else
                {
                    if (@event.Damage == 0)
                    {
                        damageSentence = ToUppercaseFirst(victim) + " seems unaffected.";
                    }
                    else
                    {
                        damageSentence = Format(" ({0} pts.)", @event.Damage);
                    }
                }

                return attackSentence + damageSentence;
            }
            else
            {
                return Format("{0} tries {1} {2}, but misses.", ToUppercaseFirst(attacker),
                    ToVerb(@event.AttackType, EnglishVerbForm.Infinitive, @event.Sensor != @event.Attacker), victim);
            }
        }

        public virtual string ToString(ItemDropEvent @event)
        {
            var dropper = @event.Sensor == @event.Dropper
                ? "you"
                : @event.DropperSensed.HasFlag(SenseType.Sight)
                    ? ToString(@event.Dropper)
                    : "something";

            return Format("{0} drops {1}.", ToUppercaseFirst(dropper), ToString(@event.Item));
        }

        public virtual string ToString(ItemPickUpEvent @event)
        {
            var dropper = @event.Sensor == @event.Picker
                ? "you"
                : @event.PickerSensed.HasFlag(SenseType.Sight)
                    ? ToString(@event.Picker)
                    : "something";

            return Format("{0} picks up {1}.", ToUppercaseFirst(dropper), ToString(@event.Item));
        }

        public virtual string ToString(DeathEvent @event)
        {
            string message;

            if (@event.Sensor == @event.Deceased)
            {
                message = "You die";
            }
            else
            {
                var deceased = @event.DeceasedSensed.HasFlag(SenseType.Sight) ? ToString(@event.Deceased) : "something";
                message = Format("{0} dies", ToUppercaseFirst(deceased));
            }

            if (@event.Corpse != null)
            {
                message += " leaving behind a corpse";
            }

            return message + ".";
        }

        public virtual string ToString(ItemConsumptionEvent @event)
        {
            var @object = @event.ObjectSensed.HasFlag(SenseType.Sight) ? @event.Object.Name : "something";
            if (@event.Sensor == @event.Consumer)
            {
                return Format("You eat a {0}.", @object);
            }
            var consumer = @event.ConsumerSensed.HasFlag(SenseType.Sight) ? ToString(@event.Consumer) : "something";
            return Format("{0} eats a {1}.", consumer, @object);
        }

        #endregion

        #region Interface messages

        public virtual string Welcome(PlayerCharacter character)
        {
            return Format("Welcome to the {0}, {1}!", character.Level.Name, ToString(character));
        }

        public virtual string UnableToMove(Direction direction)
        {
            return Format("Can't move {0}.", ToString(direction));
        }

        #endregion

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }
    }
}