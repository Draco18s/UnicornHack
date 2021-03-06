using System.Collections.Generic;
using System.Linq;
using UnicornHack.Data.Creatures;
using UnicornHack.Data.Items;
using UnicornHack.Generation;
using UnicornHack.Primitives;
using UnicornHack.Services.English;
using UnicornHack.Services.LogEvents;
using UnicornHack.Systems.Abilities;
using UnicornHack.Systems.Effects;
using UnicornHack.Utils.DataStructures;
using Xunit;

namespace UnicornHack.Systems.Knowledge
{
    public class LoggingSystemTest
    {
        // TODO: Test explosion

        [Fact]
        public void AttackEvent()
        {
            var level = TestHelper.BuildLevel(@"
...
...
...");
            var demogorgon = CreatureData.Demogorgon.Instantiate(level, new Point(0, 0));

            var nymph = CreatureData.WaterNymph.Instantiate(level, new Point(0, 1));

            var playerEntity = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(1, 1));
            playerEntity.Position.Heading = Direction.West;
            var manager = playerEntity.Manager;

            ItemData.LongSword.Instantiate(playerEntity);

            var player2Entity = PlayerRace.InstantiatePlayer("Cudley", Sex.Female, level.Entity, new Point(1, 0));
            player2Entity.Position.Heading = Direction.North;

            manager.Queue.ProcessQueue(manager);

            var swordEntity = manager.EntityItemsToContainerRelationship[playerEntity.Id]
                .Single(e => e.Item.TemplateName == ItemData.LongSword.Name);
            var equipMessage = manager.ItemUsageSystem.CreateEquipItemMessage(manager);
            equipMessage.ActorEntity = playerEntity;
            equipMessage.ItemEntity = swordEntity;
            equipMessage.Slot = EquipmentSlot.GraspBothMelee;

            manager.Enqueue(equipMessage);
            manager.Queue.ProcessQueue(manager);
            playerEntity.Player.LogEntries.Clear();
            player2Entity.Player.LogEntries.Clear();

            Verify(demogorgon, nymph, playerEntity, player2Entity,
                manager.AbilitiesToAffectableRelationship[demogorgon.Id].ElementAt(3), success: true,
                "The Demogorgon stings the water nymph. (18 pts.)",
                "The Demogorgon stings something.",
                manager);

            Verify(demogorgon, nymph, playerEntity, player2Entity,
                manager.AbilitiesToAffectableRelationship[demogorgon.Id].ElementAt(3), success: false,
                "The Demogorgon tries to sting the water nymph, but misses.",
                "The Demogorgon tries to sting something, but misses.",
                manager);

            Verify(playerEntity, demogorgon, playerEntity, player2Entity,
                manager.AbilitiesToAffectableRelationship[playerEntity.Id]
                    .Single(a => a.Ability.Slot == AbilitySlottingSystem.DefaultMeleeAttackSlot), success: true,
                "You slash the Demogorgon. (89 pts.)",
                "Dudley slashes the Demogorgon. (89 pts.)",
                manager);

            Verify(playerEntity, demogorgon, playerEntity, player2Entity,
                manager.AbilitiesToAffectableRelationship[playerEntity.Id]
                    .Single(a => a.Ability.Slot == AbilitySlottingSystem.DefaultMeleeAttackSlot), success: false,
                "You try to slash the Demogorgon, but miss.",
                "Dudley tries to slash the Demogorgon, but misses.",
                manager);

            Verify(demogorgon, playerEntity, playerEntity, player2Entity,
                manager.AbilitiesToAffectableRelationship[demogorgon.Id].ElementAt(2), success: true,
                "The Demogorgon casts a spell at you! You are unaffected.",
                "The Demogorgon casts a spell at Dudley. He is unaffected.",
                manager);

            Verify(demogorgon, playerEntity, playerEntity, player2Entity,
                manager.AbilitiesToAffectableRelationship[demogorgon.Id].ElementAt(2), success: false,
                "The Demogorgon tries to cast a spell at you, but misses.",
                "The Demogorgon tries to cast a spell at Dudley, but misses.",
                manager);

            Verify(demogorgon, playerEntity, player2Entity, SenseType.Sight, SenseType.Sight, AbilityAction.Spell,
                manager, null,
                expectedMessage: "The Demogorgon tries to cast a spell at Dudley, but misses.");

            Verify(demogorgon, nymph, playerEntity, SenseType.Sound, SenseType.Sight, AbilityAction.Sting, manager, 11,
                expectedMessage: "You hear a noise.");

            Verify(nymph, playerEntity, playerEntity, SenseType.Sight | SenseType.Touch,
                SenseType.Sight | SenseType.Touch,
                AbilityAction.Punch, manager, 11,
                expectedMessage: "The water nymph punches you! [11 pts.]");

            Verify(nymph, playerEntity, playerEntity, SenseType.Sight | SenseType.Touch,
                SenseType.Sight | SenseType.Touch,
                AbilityAction.Spit, manager, null,
                expectedMessage: "The water nymph tries to spit at you, but misses.");

            Verify(playerEntity, demogorgon, playerEntity, SenseType.Sight | SenseType.Touch, SenseType.Sight,
                AbilityAction.Hug, manager, 11,
                expectedMessage: "You squeeze the Demogorgon. (11 pts.)");

            Verify(playerEntity, demogorgon, playerEntity, SenseType.Sight | SenseType.Touch,
                SenseType.Telepathy | SenseType.Touch,
                AbilityAction.Trample, manager, null,
                expectedMessage: "You try to trample the Demogorgon, but miss.");

            Verify(demogorgon, demogorgon, playerEntity, SenseType.Sight, SenseType.Sight, AbilityAction.Claw, manager,
                11,
                expectedMessage: "The Demogorgon claws himself. (11 pts.)");

            Verify(playerEntity, playerEntity, playerEntity, SenseType.Sight | SenseType.Touch,
                SenseType.Sight | SenseType.Touch,
                AbilityAction.Kick, manager, 12,
                expectedMessage: "You kick yourself! [12 pts.]");

            Verify(nymph, playerEntity, playerEntity, SenseType.Sight | SenseType.Sound,
                SenseType.Sight | SenseType.Touch,
                AbilityAction.Scream, manager, 0,
                expectedMessage: "The water nymph screams at you! You are unaffected.");

            Verify(nymph, nymph, playerEntity, SenseType.Sight, SenseType.Sight, AbilityAction.Scream, manager, 0,
                expectedMessage: "The water nymph screams at herself. She is unaffected.");

            Verify(nymph, nymph, playerEntity, SenseType.Sound, SenseType.None, AbilityAction.Scream, manager, null,
                expectedMessage: "You hear a scream.");

            var dagger = ItemData.Dagger.Instantiate(playerEntity.Manager).Referenced;

            Verify(playerEntity, null, playerEntity, SenseType.Touch | SenseType.Telepathy, SenseType.Sight,
                AbilityAction.Slash, manager, null,
                weapon: dagger,
                expectedMessage: "You slash the air.");

            var bow = ItemData.Shortbow.Instantiate(playerEntity.Manager).Referenced;
            var arrow = ItemData.Arrow.Instantiate(playerEntity.Manager).Referenced;

            Verify(nymph, playerEntity, playerEntity, SenseType.Sight, SenseType.None, AbilityAction.Shoot, manager,
                null, weapon: bow,
                expectedMessage: null);

            Verify(nymph, playerEntity, playerEntity, SenseType.None, SenseType.Sight, AbilityAction.Hit, manager, 11,
                weapon: arrow,
                expectedMessage: "Something hits you! [11 pts.]");

            Verify(nymph, playerEntity, playerEntity, SenseType.SoundDistant, SenseType.None, AbilityAction.Shoot,
                manager, null,
                weapon: bow,
                expectedMessage: null);

            Verify(nymph, playerEntity, playerEntity, SenseType.Sight, SenseType.Sight, AbilityAction.Hit, manager,
                null, weapon: arrow,
                expectedMessage: "An arrow misses you.");

            Verify(playerEntity, demogorgon, playerEntity, SenseType.Sight, SenseType.None, AbilityAction.Shoot,
                manager, null, weapon: bow,
                expectedMessage: null);

            Verify(playerEntity, demogorgon, playerEntity, SenseType.Sight, SenseType.Sight, AbilityAction.Hit, manager,
                11, weapon: arrow,
                expectedMessage: "An arrow hits the Demogorgon. (11 pts.)");

            Verify(nymph, demogorgon, playerEntity, SenseType.Sound, SenseType.None, AbilityAction.Shoot, manager, null,
                weapon: bow,
                expectedMessage: "You hear a noise.");

            Verify(nymph, demogorgon, playerEntity, SenseType.Sight, SenseType.Sight, AbilityAction.Hit, manager, 2,
                weapon: arrow,
                expectedMessage: "An arrow hits the Demogorgon. (2 pts.)");

            var throwingKnife = ItemData.ThrowingKnife.Instantiate(playerEntity.Manager).Referenced;

            Verify(nymph, playerEntity, playerEntity, SenseType.Sound, SenseType.None, AbilityAction.Throw,
                manager,
                null,
                weapon: throwingKnife,
                expectedMessage: null);

            Verify(nymph, playerEntity, playerEntity, SenseType.Sight, SenseType.Sight, AbilityAction.Hit, manager,
                11,
                weapon: throwingKnife,
                expectedMessage: "A throwing knife hits you! [11 pts.]");

            Verify(playerEntity, null, playerEntity, SenseType.None, SenseType.None, AbilityAction.Hit, manager, null,
                weapon: throwingKnife,
                expectedMessage: null);

            Verify(nymph, demogorgon, playerEntity, SenseType.Sound, SenseType.None, AbilityAction.Throw, manager, null,
                weapon: throwingKnife,
                expectedMessage: "You hear a noise.");

            Verify(nymph, demogorgon, playerEntity, SenseType.None, SenseType.SoundDistant, AbilityAction.Hit, manager,
                2,
                weapon: throwingKnife,
                expectedMessage: "You hear a distant noise.");
        }

        private static void Verify(
            GameEntity attacker,
            GameEntity victim,
            GameEntity player1,
            GameEntity player2,
            GameEntity ability,
            bool success,
            string expectedMessage1,
            string expectedMessage2,
            GameManager manager)
        {
            var activationMessage = manager.AbilityActivationSystem.CreateActivateAbilityMessage(manager);
            activationMessage.AbilityEntity = ability;
            activationMessage.ActivatorEntity = attacker;
            activationMessage.TargetEntity = victim;
            ability.Ability.CooldownTick = null;

            manager.Enqueue(activationMessage);

            ((TestRandom)manager.Game.Random).EnqueueNextBool(success);
            manager.Queue.ProcessQueue(manager);

            Assert.Equal(expectedMessage1,
                player1.Player.LogEntries.Single().Message);
            player1.Player.LogEntries.Clear();
            Assert.Equal(expectedMessage2,
                player2.Player.LogEntries.Single().Message);
            player2.Player.LogEntries.Clear();
        }

        private void Verify(
            GameEntity attacker,
            GameEntity victim,
            GameEntity sensor,
            SenseType attackerSensed,
            SenseType victimSensed,
            AbilityAction abilityAction,
            GameManager manager,
            int? damage,
            GameEntity weapon = null,
            string expectedMessage = "")
        {
            var languageService = manager.Game.Services.Language;

            var appliedEffects = new List<GameEntity>();
            if (damage.HasValue)
            {
                using (var damageEffectEntity = manager.CreateEntity())
                {
                    var entity = damageEffectEntity.Referenced;

                    var appliedEffect = manager.CreateComponent<EffectComponent>(EntityComponent.Effect);
                    appliedEffect.Amount = damage.Value;
                    appliedEffect.EffectType = EffectType.PhysicalDamage;
                    appliedEffect.AffectedEntityId = victim.Id;

                    entity.Effect = appliedEffect;

                    appliedEffects.Add(entity);
                }

                if (weapon != null)
                {
                    using (var weaponEffectEntity = manager.CreateEntity())
                    {
                        var entity = weaponEffectEntity.Referenced;

                        var appliedEffect = manager.CreateComponent<EffectComponent>(EntityComponent.Effect);
                        appliedEffect.Amount = damage.Value;
                        appliedEffect.EffectType = EffectType.Activate;
                        appliedEffect.TargetEntityId = weapon.Id;
                        appliedEffect.AffectedEntityId = victim.Id;

                        entity.Effect = appliedEffect;

                        appliedEffects.Add(entity);
                    }
                }
            }

            var attackEvent = new AttackEvent(sensor, attacker, victim, attackerSensed, victimSensed,
                appliedEffects, abilityAction, weapon,
                ranged: weapon != null && (weapon.Item.Type & ItemType.WeaponRanged) != 0, hit: damage.HasValue);

            Assert.Equal(expectedMessage, languageService.GetString(attackEvent));
        }

        [Fact]
        public void DeathEvent()
        {
            var level = TestHelper.BuildLevel();
            var blob = CreatureData.AcidBlob.Instantiate(level, new Point(0, 0));
            var player = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 2));
            var manager = level.Entity.Manager;
            var languageService = manager.Game.Services.Language;

            Assert.Equal("The acid blob dies.",
                languageService.GetString(new DeathEvent(player, blob, SenseType.Sight)));

            Assert.Equal("You die!",
                languageService.GetString(new DeathEvent(player, player, SenseType.Sight | SenseType.Touch)));
        }

        [Fact]
        public void ItemEquipmentEvent()
        {
            var level = TestHelper.BuildLevel();
            var armor = ItemData.MailArmor.Instantiate(level.Entity.Manager).Referenced;
            var nymph = CreatureData.WaterNymph.Instantiate(level, new Point(0, 1));
            var player = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 2));
            var manager = level.Entity.Manager;
            var languageService = manager.Game.Services.Language;

            Assert.Equal("The water nymph equips something on the torso.",
                languageService.GetString(new ItemEquipmentEvent(
                    player, nymph, armor, SenseType.Sight, SenseType.Sound, EquipmentSlot.Torso)));

            Assert.Equal("Something equips a mail armor.", languageService.GetString(new ItemEquipmentEvent(
                player, nymph, armor, SenseType.Sound, SenseType.Sight, EquipmentSlot.Torso)));

            Assert.Equal("You equip something on the torso.", languageService.GetString(new ItemEquipmentEvent(
                player, player, armor, SenseType.Sight | SenseType.Touch, SenseType.Touch, EquipmentSlot.Torso)));

            var sword = ItemData.LongSword.Instantiate(level.Entity.Manager).Referenced;

            Assert.Equal("You equip the long sword in the main hand for melee.", languageService.GetString(new ItemEquipmentEvent(
                player, player, sword, SenseType.Sight | SenseType.Touch,
                SenseType.Sight | SenseType.Touch, EquipmentSlot.GraspPrimaryMelee)));
        }

        [Fact]
        public void ItemUnequipmentEvent()
        {
            var level = TestHelper.BuildLevel();
            var armor = ItemData.MailArmor.Instantiate(level.Entity.Manager).Referenced;
            var nymph = CreatureData.WaterNymph.Instantiate(level, new Point(0, 1));
            var player = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 2));
            var manager = level.Entity.Manager;
            var languageService = manager.Game.Services.Language;

            Assert.Equal("The water nymph unequips something.", languageService.GetString(new ItemEquipmentEvent(
                player, nymph, armor, SenseType.Sight, SenseType.Sound, EquipmentSlot.None)));

            Assert.Equal("Something unequips a mail armor.", languageService.GetString(new ItemEquipmentEvent(
                player, nymph, armor, SenseType.Sound, SenseType.Sight, EquipmentSlot.None)));

            Assert.Equal("You unequip something.", languageService.GetString(new ItemEquipmentEvent(
                player, player, armor, SenseType.Sight | SenseType.Touch, SenseType.Touch, EquipmentSlot.None)));

            var sword = ItemData.LongSword.Instantiate(level.Entity.Manager).Referenced;

            Assert.Equal("You unequip the long sword.", languageService.GetString(new ItemEquipmentEvent(
                player, player, sword, SenseType.Sight | SenseType.Touch,
                SenseType.Sight | SenseType.Touch, EquipmentSlot.None)));
        }

        [Fact]
        public void ItemConsumptionEvent()
        {
            var level = TestHelper.BuildLevel();
            var flask = ItemData.FlaskOfHealing.Instantiate(level.Entity.Manager).Referenced;
            var potion = ItemData.PotionOfExperience.Instantiate(level.Entity.Manager).Referenced;
            var blob = CreatureData.AcidBlob.Instantiate(level, new Point(0, 0));
            var player = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 2));
            var manager = level.Entity.Manager;
            var languageService = manager.Game.Services.Language;

            Assert.Equal("The acid blob drinks from a flask of healing.", languageService.GetString(
                new ItemActivationEvent(player, flask, blob, blob,
                    SenseType.Sight | SenseType.Sound, SenseType.Sight | SenseType.Sound,
                    SenseType.Sight | SenseType.Sound, consumed: false, successful: true)));

            Assert.Equal("You drink from the flask of healing.", languageService.GetString(
                new ItemActivationEvent(player, flask, player, player,
                    SenseType.Sight | SenseType.Sound, SenseType.Sight | SenseType.Touch,
                    SenseType.Sight | SenseType.Touch, consumed: false, successful: true)));

            Assert.Equal("You drink the potion of experience.", languageService.GetString(
                new ItemActivationEvent(player, potion, player, player,
                    SenseType.Sight | SenseType.Sound, SenseType.Sight | SenseType.Touch,
                    SenseType.Sight | SenseType.Touch, consumed: true, successful: true)));
        }

        [Fact]
        public void ItemPickUpEvent()
        {
            var level = TestHelper.BuildLevel();
            var coins = ItemData.GoldCoin.Instantiate(level.Entity.Manager).Referenced;
            var blob = CreatureData.AcidBlob.Instantiate(level, new Point(0, 0));
            var player = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 2));
            var manager = level.Entity.Manager;
            var languageService = manager.Game.Services.Language;

            Assert.Equal("The acid blob picks up 11 gold coins.", languageService.GetString(new ItemPickUpEvent(
                player, blob, coins, 11, SenseType.Sight | SenseType.Sound, SenseType.Sight | SenseType.Sound)));

            Assert.Equal("You pick up 11 gold coins.", languageService.GetString(new ItemPickUpEvent(
                player, player, coins, 11, SenseType.Sight | SenseType.Sound, SenseType.Sight | SenseType.Sound)));
        }

        [Fact]
        public void ItemDropEvent()
        {
            var level = TestHelper.BuildLevel();
            var coins = ItemData.GoldCoin.Instantiate(level.Entity.Manager).Referenced;
            var blob = CreatureData.AcidBlob.Instantiate(level, new Point(0, 0));
            var player = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 2));
            var manager = level.Entity.Manager;
            var languageService = manager.Game.Services.Language;

            Assert.Equal("The acid blob drops 11 gold coins.", languageService.GetString(new ItemDropEvent(
                player, blob, coins, 11, SenseType.Sight | SenseType.Sound, SenseType.Sight | SenseType.Sound)));

            Assert.Equal("You drop 11 gold coins.", languageService.GetString(new ItemDropEvent(
                player, player, coins, 11, SenseType.Sight | SenseType.Sound, SenseType.Sight | SenseType.Sound)));
        }

        [Fact]
        public void LeveledUpEvent()
        {
            var level = TestHelper.BuildLevel();
            var player1 = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 0));
            var player2 = PlayerRace.InstantiatePlayer("Cudley", Sex.Female, level.Entity, new Point(0, 1));
            var manager = level.Entity.Manager;
            var languageService = manager.Game.Services.Language;

            Assert.Equal("You level up! You gain 2 SP 1 TP 0 MP.",
                languageService.GetString(new LeveledUpEvent(
                    player1, player1, manager.RacesToBeingRelationship[player1.Id].Single().Value.Race, 2, 1, 0)));

            Assert.Equal("Cudley levels up! She gains 3 SP 2 TP 1 MP.",
                languageService.GetString(new LeveledUpEvent(
                    player1, player2, manager.RacesToBeingRelationship[player2.Id].Single().Value.Race, 3, 2, 1)));
        }

        [Fact]
        public void Welcome()
        {
            var level = TestHelper.BuildLevel();
            var player = PlayerRace.InstantiatePlayer("Conan the Barbarian", Sex.Male, level.Entity, new Point(0, 0));
            var manager = level.Entity.Manager;
            var languageService = manager.Game.Services.Language;

            var message = languageService.Welcome(player);

            Assert.Equal("Welcome to the test branch, Conan the Barbarian!", message);
        }

        protected EnglishLanguageService CreateLanguageService() => new EnglishLanguageService();
    }
}
