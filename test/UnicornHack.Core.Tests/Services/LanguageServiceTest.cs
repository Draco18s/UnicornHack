﻿using UnicornHack.Models.GameDefinitions;
using UnicornHack.Models.GameState;
using UnicornHack.Models.GameState.Events;
using UnicornHack.Services;
using Xunit;

namespace UnicornHack.Core.Services
{
    public class LanguageServiceTest
    {
        [Fact]
        public void AttackEventTest()
        {
            var newt = new Monster(ActorVariant.Newt);
            var nymph = new Monster(ActorVariant.NymphWater) { Sex = Sex.Female };
            var rodney = new Monster(ActorVariant.Rodney) { Sex = Sex.None };
            var player = new PlayerCharacter(ActorVariant.Human) { GivenName = "Dudley", Sex = Sex.Male };
            var player2 = new PlayerCharacter(ActorVariant.Human) { GivenName = "Cudley", Sex = Sex.Male };

            Verify(newt, nymph, player, SenseType.Sight, SenseType.Sight, AttackType.Bite, 11,
                expectedMessage: "The newt bites the water nymph. (11 pts.)");

            Verify(player, rodney, player2, SenseType.Sight, SenseType.Sight, AttackType.Sting, 11,
                expectedMessage: "Dudley stings Wizard of Yendor. (11 pts.)");

            Verify(newt, nymph, player, SenseType.Sound, SenseType.Sight, AttackType.Sting, 11,
                expectedMessage: "You hear some noises.");

            Verify(newt, nymph, player, SenseType.Sight, SenseType.SoundDistant | SenseType.Danger |SenseType.Touch, AttackType.Headbutt, 11,
                expectedMessage: "The newt headbutts something.");

            Verify(nymph, player, player, SenseType.Sight | SenseType.Touch, SenseType.Sight | SenseType.Touch, AttackType.Punch, 11,
                expectedMessage: "The water nymph punches! [11 pts.]");

            Verify(nymph, player, player, SenseType.Sight | SenseType.Touch, SenseType.Sight | SenseType.Touch, AttackType.Spit, null,
                expectedMessage: "The water nymph tries to spit at you, but misses.");

            Verify(player, newt, player, SenseType.Sight | SenseType.Touch, SenseType.Sight, AttackType.Hug, 11,
                expectedMessage: "You squeeze the newt. (11 pts.)");

            Verify(player, newt, player, SenseType.Sight | SenseType.Touch, SenseType.Telepathy | SenseType.Touch, AttackType.Trample, null,
                expectedMessage: "You try to trample the newt, but miss.");

            Verify(newt, newt, player, SenseType.Sight, SenseType.Sight, AttackType.Claw, 11,
                expectedMessage: "The newt claws itself. (11 pts.)");

            Verify(player, player, player, SenseType.Sight | SenseType.Touch, SenseType.Sound | SenseType.Touch, AttackType.Kick, 12,
                expectedMessage: "You kick yourself! [12 pts.]");

            Verify(nymph, nymph, player, SenseType.Sight, SenseType.Sight, AttackType.Scream, 0,
                expectedMessage: "The water nymph screams at herself. She seems unaffected.");

            Verify(nymph, nymph, player, SenseType.Sound, SenseType.None, AttackType.Scream, null,
                expectedMessage: "You hear a scream.");

            //Verify(nymph, player, player, SenseType.Sight, SenseType.Sight, AttackType.Weapon, 11, weapon: Bow, projectile: Arrow,
            //    expectedMessage: "The water nymph shoots an arrow. You are hit by an arrow! [11 pts.]");

            //Verify(player, newt, player, SenseType.Sight, SenseType.Sight, AttackType.Weapon, 11, weapon: Bow, projectile: Arrow,
            //    expectedMessage: "The arrow hits the newt. (11 pts.)");
        }

        private void Verify(
            Actor attacker,
            Actor victim,
            PlayerCharacter sensor,
            SenseType attackerSensed,
            SenseType victimSensed,
            AttackType attackType,
            int? damage,
            Item weapon = null,
            Item projectile = null,
            string expectedMessage = "")
        {
            var languageService = CreateLanguageService();
            var attackEvent = new AttackEvent
            {
                Attacker = attacker,
                Victim = victim,
                Sensor = sensor,
                AttackerSensed = attackerSensed,
                VictimSensed = victimSensed,
                AttackType = attackType,
                Hit = damage.HasValue,
                Weapon = weapon,
                Projectile = projectile
            };

            if (damage.HasValue)
            {
                attackEvent.Damage = damage.Value;
            }

            Assert.Equal(expectedMessage, languageService.ToString(attackEvent));
        }

        [Fact]
        public void Welcome()
        {
            var languageService = CreateLanguageService();

            var message = languageService.Welcome(
                new PlayerCharacter(ActorVariant.Human) { GivenName = "Conan the Barbarian", Level = new Level {Name = "Dungeon of Fun"}});

            Assert.Equal("Welcome to the Dungeon of Fun, Conan the Barbarian!", message);
        }

        protected LanguageService CreateLanguageService() => new LanguageService();
    }
}