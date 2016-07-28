using System;
using System.ComponentModel;
using System.Globalization;
using SimpleNLG;
using UnicornHack.Models.GameDefinitions;
using UnicornHack.Models.GameState;
using UnicornHack.Models.GameState.Events;
using UnicornHack.Utils;

namespace UnicornHack.Services
{
    public class LanguageService
    {
        private static readonly Lexicon Lexicon = Lexicon.getDefaultLexicon();
        private static readonly NLGFactory NlgFactory = new NLGFactory(Lexicon);
        private static readonly Realiser Realiser = new Realiser(Lexicon);

        protected virtual CultureInfo Culture { get; } = new CultureInfo(name: "en-US");

        public virtual string Format(string format, params object[] arguments)
        {
            return string.Format(Culture, format, arguments);
        }

        #region Natural language helpers
        
        private string Exclame(string sentence) => sentence.TrimEnd('.') + "!";

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
                                       ? dropedItemString
                                       : dropedItemString);
            }

            return dropedItemString;
        }

        private string ToVerb(AttackType attackType, Form form, bool singularThirdPerson)
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


            var attacker = NlgFactory.createNounPhrase(
                @event.Sensor == @event.Attacker
                    ? "you"
                    : @event.AttackerSensed.HasFlag(SenseType.Sight)
                        ? ToString(@event.Attacker, definiteDeterminer: true)
                        : "something");

            var victim = NlgFactory.createNounPhrase(
                @event.Attacker == @event.Victim
                        ? null
                        : @event.VictimSensed.HasFlag(SenseType.Sight)
                            ? ToString(@event.Victim, definiteDeterminer: true)
                            : "something");

            var attack = ToVerb(@event.AttackType, Form.NORMAL, false);

            if (@event.Hit)
            {
                var attackClause = NlgFactory.createClause(attacker, attack, victim);
                //var verbPhrase = NlgFactory.createVerbPhrase(attack);
                //verbPhrase.setFeature(Feature.FORM.ToString(), Form.BARE_INFINITIVE);

                var attackSentence = Realiser.realiseSentence(attackClause);
                if (@event.Sensor == @event.Victim)
                {
                    attackSentence = Exclame(attackSentence);
                }

                var damageSentence = "";
                if (!@event.VictimSensed.HasFlag(SenseType.Sight))
                {
                    return attackSentence;
                }
                if (@event.Sensor == @event.Victim)
                {
                    if (@event.Damage == 0)
                    {
                        var damageClause = NlgFactory.createClause("you", "is", "unaffected");
                        damageSentence = Realiser.realiseSentence(damageClause);
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
                        var damageClause = NlgFactory.createClause(victim, "seem", "unaffected");
                        damageSentence = Realiser.realiseSentence(damageClause);
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
                var attackVerb = NlgFactory.createVerbPhrase(attack);

                var attackClause = NlgFactory.createClause();
                attackClause.setVerbPhrase(attackVerb);
                attackClause.setObject(victim);

                attackClause.setFeature(Feature.FORM.ToString(), Form.INFINITIVE);

                var missClause = NlgFactory.createClause(attacker, "try", attackClause);
                //, but misses.

                return Realiser.realiseSentence(missClause);
            }
        }

        public virtual string ToString(ItemDropEvent @event)
        {
            var dropper = @event.Sensor == @event.Dropper
                ? "you"
                : @event.DropperSensed.HasFlag(SenseType.Sight)
                    ? ToString((dynamic)@event.Dropper)
                    : "something";

            return Format("{0} drops {1}.", ToUppercaseFirst(dropper), ToString(@event.Item));
        }

        public virtual string ToString(ItemPickUpEvent @event)
        {
            var dropper = @event.Sensor == @event.Picker
                ? "you"
                : @event.PickerSensed.HasFlag(SenseType.Sight)
                    ? ToString((dynamic)@event.Picker)
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
                var deceased = @event.DeceasedSensed.HasFlag(SenseType.Sight) ? ToString((dynamic)@event.Deceased) : "something";
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
            var consumer = @event.ConsumerSensed.HasFlag(SenseType.Sight) ? ToString((dynamic)@event.Consumer) : "something";
            return Format("{0} eats a {1}.", consumer, @object);
        }

        #endregion

        #region Interface messages

        public virtual string Welcome(PlayerCharacter character)
        {
            var verbPhrase = NlgFactory.createVerbPhrase("welcome to");
            verbPhrase.setFeature(Feature.FORM.ToString(), Form.BARE_INFINITIVE);

            var place = NlgFactory.createNounPhrase("the", character.Level.Name);
            verbPhrase.setIndirectObject(place);

            var vocative = NlgFactory.createNounPhrase(character.GivenName);
            vocative.setFeature(Feature.APPOSITIVE.ToString(), true);
            verbPhrase.addPostModifier(vocative);

            return Exclame(Realiser.realiseSentence(verbPhrase));
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
