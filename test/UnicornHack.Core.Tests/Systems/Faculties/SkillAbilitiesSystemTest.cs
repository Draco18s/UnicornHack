﻿using System.Linq;
using UnicornHack.Data.Abilities;
using UnicornHack.Data.Items;
using UnicornHack.Generation;
using UnicornHack.Primitives;
using UnicornHack.Systems.Abilities;
using UnicornHack.Utils.DataStructures;
using Xunit;

namespace UnicornHack.Systems.Faculties
{
    public class SkillAbilitiesSystemTest
    {
        [Fact]
        public void Skills_add_abilities()
        {
            var level = TestHelper.BuildLevel(".");
            var playerEntity = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 0));
            ItemData.SkillbookOfWaterSourcery.Instantiate(playerEntity);
            var manager = playerEntity.Manager;

            manager.Queue.ProcessQueue(manager);

            Assert.Null(manager.AffectableAbilitiesIndex[(playerEntity.Id, "ice shard")]);

            var firstWaterBookAbility =
                manager.AffectableAbilitiesIndex[
                    (playerEntity.Id, ItemData.SkillbookOfWaterSourcery.Name + ": Consult")];
            var firstWaterBook = manager.EntityItemsToContainerRelationship[playerEntity.Id].Single();
            ItemData.SkillbookOfWaterSourcery.Instantiate(playerEntity);
            ItemData.SkillbookOfConjuration.Instantiate(playerEntity);
            ActivateAbility(firstWaterBookAbility, 0, manager);

            manager.Queue.ProcessQueue(manager);

            Assert.True(manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.IceShard.Name)].Ability
                .IsUsable);

            var conjurationBookAbility = manager.AbilitiesToAffectableRelationship[playerEntity.Id]
                .First(a => a.Ability.Name == ItemData.SkillbookOfConjuration.Name + ": Consult");
            ActivateAbility(conjurationBookAbility, 2, manager);

            manager.Queue.ProcessQueue(manager);

            Assert.True(manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.IceShard.Name)].Ability
                .IsUsable);

            Assert.Equal(1,
                manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.WaterSourcery.Name)].Ability.Level);

            playerEntity.Player.SkillPoints = 5;
            manager.SkillAbilitiesSystem.BuyAbilityLevel(AbilityData.WaterSourcery, playerEntity);

            manager.Queue.ProcessQueue(manager);

            Assert.Equal(2,
                manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.WaterSourcery.Name)].Ability.Level);
            Assert.Equal(1,
                manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.Conjuration.Name)].Ability.Level);
            Assert.True(manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.IceShard.Name)].Ability
                .IsUsable);

            DeactivateAbility(conjurationBookAbility, manager);

            var moveItemMessage = manager.ItemMovingSystem.CreateMoveItemMessage(manager);
            moveItemMessage.ItemEntity = firstWaterBook;
            moveItemMessage.TargetCell = new Point(0, 0);
            moveItemMessage.TargetLevelEntity = level.Entity;

            manager.Enqueue(moveItemMessage);
            manager.Queue.ProcessQueue(manager);

            Assert.Null(firstWaterBookAbility.Manager);
            Assert.Equal(1,
                manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.WaterSourcery.Name)].Ability.Level);
            Assert.Null(manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.Conjuration.Name)]);
            Assert.True(manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.IceShard.Name)].Ability
                .IsUsable);

            var deactivateMessage = manager.AbilityActivationSystem.CreateDeactivateAbilityMessage(manager);
            deactivateMessage.AbilityEntity =
                manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.WaterSourcery.Name)];
            deactivateMessage.ActivatorEntity = playerEntity;
            manager.Enqueue(deactivateMessage);

            manager.Queue.ProcessQueue(manager);

            manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.WaterSourcery.Name)].Ability = null;

            manager.Queue.ProcessQueue(manager);

            Assert.Null(manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.WaterSourcery.Name)]);
            Assert.Null(manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.Conjuration.Name)]);
            Assert.False(manager.AffectableAbilitiesIndex[(playerEntity.Id, AbilityData.IceShard.Name)]?.Ability
                             ?.IsUsable == true);
        }

        [Fact]
        public void Default_attacks_are_slotted_when_weapons_are_equipped()
        {
            var level = TestHelper.BuildLevel(".");
            var playerEntity = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 0));
            var manager = playerEntity.Manager;
            ItemData.LongSword.Instantiate(playerEntity);
            ItemData.FireStaff.Instantiate(playerEntity);

            manager.Queue.ProcessQueue(manager);

            Assert.Same(AbilityData.DoubleMeleeAttack.Name,
                manager.SlottedAbilitiesIndex[(playerEntity.Id, AbilitySlottingSystem.DefaultMeleeAttackSlot)].Ability.Name);
            Assert.Null(
                manager.SlottedAbilitiesIndex[(playerEntity.Id, AbilitySlottingSystem.DefaultRangedAttackSlot)]);
            Assert.Equal(1, manager.AbilitiesToAffectableRelationship[playerEntity.Id].Select(a => a.Ability)
                .Count(a => a.IsUsable && (a.Activation & ActivationType.Slottable) != 0));

            var swordEntity = manager.EntityItemsToContainerRelationship[playerEntity.Id]
                .Single(e => e.Item.TemplateName == ItemData.LongSword.Name);
            var equipMessage = manager.ItemUsageSystem.CreateEquipItemMessage(manager);
            equipMessage.ActorEntity = playerEntity;
            equipMessage.ItemEntity = swordEntity;
            equipMessage.Slot = EquipmentSlot.GraspBothMelee;

            manager.Enqueue(equipMessage);

            var staffEntity = manager.EntityItemsToContainerRelationship[playerEntity.Id]
                .Single(e => e.Item.TemplateName == ItemData.FireStaff.Name);
            equipMessage = manager.ItemUsageSystem.CreateEquipItemMessage(manager);
            equipMessage.ActorEntity = playerEntity;
            equipMessage.ItemEntity = staffEntity;
            equipMessage.Slot = EquipmentSlot.GraspBothRanged;

            manager.Enqueue(equipMessage);

            manager.Queue.ProcessQueue(manager);

            Assert.Same(AbilityData.TwoHandedMeleeAttack.Name,
                manager.SlottedAbilitiesIndex[(playerEntity.Id, AbilitySlottingSystem.DefaultMeleeAttackSlot)].Ability.Name);
            Assert.Same(AbilityData.TwoHandedRangedAttack.Name,
                manager.SlottedAbilitiesIndex[(playerEntity.Id, AbilitySlottingSystem.DefaultRangedAttackSlot)].Ability.Name);
            Assert.Equal(2, manager.AbilitiesToAffectableRelationship[playerEntity.Id].Select(a => a.Ability)
                .Count(a => a.IsUsable && (a.Activation & ActivationType.Slottable) != 0));
        }

        private static GameEntity ActivateAbility(GameEntity abilityEntity, int slot, GameManager manager)
        {
            var setSlotMessage = manager.AbilitySlottingSystem.CreateSetAbilitySlotMessage(manager);
            setSlotMessage.AbilityEntity = abilityEntity;
            setSlotMessage.Slot = slot;

            manager.Enqueue(setSlotMessage);

            return abilityEntity;
        }

        private static void DeactivateAbility(GameEntity abilityEntity, GameManager manager)
        {
            var setSlotMessage = manager.AbilitySlottingSystem.CreateSetAbilitySlotMessage(manager);
            setSlotMessage.AbilityEntity = abilityEntity;

            manager.Enqueue(setSlotMessage);
        }
    }
}
