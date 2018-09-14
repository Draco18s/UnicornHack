﻿using UnicornHack.Systems.Actors;
using UnicornHack.Systems.Effects;
using UnicornHack.Systems.Time;
using UnicornHack.Utils.MessagingECS;

// ReSharper disable once CheckNamespace
namespace UnicornHack
{
    public partial class GameManager
    {
        public EntityGroup<GameEntity> TemporalEntities { get; private set; }
        public SortedUniqueEntityIndex<GameEntity, (int Tick, int Id)> TemporalEntitiesIndex { get; private set; }
        public TimeSystem TimeSystem { get; private set; }

        private void InitializeTime(SequentialMessageQueue<GameManager> queue)
        {
            TemporalEntities = CreateGroup(nameof(TemporalEntities),
                new EntityMatcher<GameEntity>().AnyOf(
                    (int)EntityComponent.AI,
                    (int)EntityComponent.Player,
                    (int)EntityComponent.Effect));

            TemporalEntitiesIndex = new SortedUniqueEntityIndex<GameEntity, (int Tick, int Id)>(
                TemporalEntities,
                new KeyValueGetter<GameEntity, (int Tick, int Id)>(
                    (entity, changes, getOldValue, matcher) =>
                    {
                        if (matcher.TryGetValue<int?>(
                            entity, (int)EntityComponent.AI, nameof(AIComponent.NextActionTick), changes, getOldValue, out var aiTick)
                            && aiTick.HasValue)
                        {
                            return ((aiTick.Value, entity.Id), true);
                        }

                        if (matcher.TryGetValue<int?>(
                            entity, (int)EntityComponent.Player, nameof(PlayerComponent.NextActionTick), changes, getOldValue, out var playerTick)
                            && playerTick.HasValue)
                        {
                            return ((playerTick.Value, entity.Id), true);
                        }

                        if (matcher.TryGetValue<int?>(
                            entity, (int)EntityComponent.Effect, nameof(EffectComponent.ExpirationTick), changes, getOldValue, out var effectTick)
                            && effectTick.HasValue)
                        {
                            return ((effectTick.Value, entity.Id), true);
                        }

                        return ((0, 0), false);
                    },
                    new PropertyMatcher()
                        .With(component => ((AIComponent)component).NextActionTick, (int)EntityComponent.AI)
                        .With(component => ((PlayerComponent)component).NextActionTick, (int)EntityComponent.Player)
                        .With(component => ((EffectComponent)component).ExpirationTick, (int)EntityComponent.Effect)
                ),
                TickComparer.Instance);

            TimeSystem = new TimeSystem();
            queue.Add(TimeSystem, TimeSystem.AdvanceTurnMessageName, 0);
        }
    }
}
