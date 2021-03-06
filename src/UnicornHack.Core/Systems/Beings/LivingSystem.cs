﻿using System;
using UnicornHack.Systems.Knowledge;
using UnicornHack.Systems.Levels;
using UnicornHack.Systems.Time;
using UnicornHack.Utils.MessagingECS;

namespace UnicornHack.Systems.Beings
{
    public class LivingSystem :
        IGameSystem<XPGainedMessage>,
        IGameSystem<PropertyValueChangedMessage<GameEntity, int>>
    {
        public const string DiedMessageName = "Died";
        public const string AttributedAbilityName = "attributed";

        public MessageProcessingResult Process(XPGainedMessage message, GameManager manager)
        {
            var player = message.Entity.Player;
            var being = message.Entity.Being;

            var hpRegenerationRate = (float)player.NextLevelXP / (being.HitPointMaximum * 4);
            var hpRegeneratingXp = message.ExperiencePoints + being.LeftoverHPRegenerationXP;
            var hpRegenerated = (int)Math.Floor(hpRegeneratingXp / hpRegenerationRate);
            being.LeftoverHPRegenerationXP = hpRegeneratingXp % hpRegenerationRate;
            being.HitPoints += hpRegenerated;

            var epRegenerationRate = (float)player.NextLevelXP / (being.EnergyPointMaximum * 4);
            var epRegeneratingXp = message.ExperiencePoints + being.LeftoverEPRegenerationXP;
            var epRegenerated = (int)Math.Floor(epRegeneratingXp / epRegenerationRate);
            being.LeftoverEPRegenerationXP = epRegeneratingXp % epRegenerationRate;
            being.EnergyPoints += epRegenerated;

            return MessageProcessingResult.ContinueProcessing;
        }

        public MessageProcessingResult Process(
            PropertyValueChangedMessage<GameEntity, int> message, GameManager manager)
        {
            var being = message.Entity.Being;
            switch (message.ChangedPropertyName)
            {
                case nameof(BeingComponent.HitPoints):
                    if (message.OldValue > 0
                        && message.NewValue <= 0)
                    {
                        EnqueueDiedMessage(message.Entity, manager);
                    }

                    break;
                case nameof(BeingComponent.HitPointMaximum):
                    if (being.HitPoints > message.NewValue)
                    {
                        being.HitPoints = message.NewValue;
                    }

                    if (message.OldValue == 1)
                    {
                        being.HitPoints = message.NewValue;

                        // TODO: Move to AI/Player system
                        // Initialize NextActionTick here so that actors don't try to act before they have HP
                        var ai = message.Entity.AI;
                        if (ai != null)
                        {
                            if (ai.NextActionTick == null)
                            {
                                ai.NextActionTick = manager.Game.CurrentTick;
                            }
                        }
                        else
                        {
                            var player = message.Entity.Player;
                            if (player != null)
                            {
                                player.NextActionTick = manager.Game.CurrentTick;
                            }
                        }
                    }

                    break;
                case nameof(BeingComponent.EnergyPointMaximum):
                    if (being.EnergyPoints > message.NewValue)
                    {
                        being.EnergyPoints = message.NewValue;
                    }

                    if (message.OldValue == 0)
                    {
                        being.EnergyPoints = message.NewValue;
                    }

                    break;
                case nameof(BeingComponent.Might):
                    var hpEffect = manager.EffectApplicationSystem.GetPropertyEffect(
                        message.Entity, nameof(BeingComponent.HitPointMaximum), AttributedAbilityName);

                    hpEffect.Amount = message.NewValue * 10;

                    break;
                case nameof(BeingComponent.Focus):
                    var epEffect = manager.EffectApplicationSystem.GetPropertyEffect(
                        message.Entity, nameof(BeingComponent.EnergyPointMaximum), AttributedAbilityName);

                    epEffect.Amount = message.NewValue * 10;

                    break;
                case nameof(BeingComponent.Speed):
                    var movementEffect = manager.EffectApplicationSystem.GetPropertyEffect(
                        message.Entity, nameof(PositionComponent.MovementDelay), AttributedAbilityName);

                    movementEffect.Amount = message.NewValue == 0
                        ? 0
                        : TimeSystem.DefaultActionDelay * 10 / message.NewValue;

                    var evasionEffect = manager.EffectApplicationSystem.GetPropertyEffect(
                        message.Entity, nameof(BeingComponent.Evasion), AttributedAbilityName);

                    evasionEffect.Amount = message.NewValue * 5;

                    break;
            }

            return MessageProcessingResult.ContinueProcessing;
        }

        private void EnqueueDiedMessage(GameEntity entity, GameManager manager)
        {
            var died = manager.Queue.CreateMessage<DiedMessage>(DiedMessageName);
            died.BeingEntity = entity;
            manager.Enqueue(died);
        }
    }
}
