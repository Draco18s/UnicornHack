﻿using UnicornHack.Systems.Beings;
using UnicornHack.Systems.Effects;
using UnicornHack.Systems.Items;
using UnicornHack.Systems.Knowledge;
using UnicornHack.Systems.Levels;
using UnicornHack.Systems.Senses;
using UnicornHack.Utils.MessagingECS;

// ReSharper disable once CheckNamespace
namespace UnicornHack
{
    public partial class GameManager
    {
        public EntityGroup<GameEntity> LevelKnowledges { get; private set; }
        public EntityRelationship<GameEntity> LevelKnowledgesToLevelRelationship { get; private set; }
        public EntityIndex<GameEntity, (int, byte, byte)> LevelKnowledgeToLevelCellIndex { get; private set; }
        public UniqueEntityRelationship<GameEntity> LevelKnowledgeToLevelEntityRelationship { get; private set; }
        public KnowledgeSystem KnowledgeSystem { get; private set; }

        private void InitializeKnowledge(SequentialMessageQueue<GameManager> queue)
        {
            Add<KnowledgeComponent>(EntityComponent.Knowledge, poolSize: 32);

            LevelKnowledges = CreateGroup(nameof(LevelKnowledges),
                new EntityMatcher<GameEntity>().AllOf((int)EntityComponent.Knowledge, (int)EntityComponent.Position));

            LevelKnowledgeToLevelCellIndex = new EntityIndex<GameEntity, (int, byte, byte)>(
                LevelKnowledges,
                new KeyValueGetter<GameEntity, (int, byte, byte)>(
                    (entity, changes, getOldValue, matcher) =>
                    {
                        if (!matcher.TryGetValue<int>(
                            entity, (int)EntityComponent.Position, nameof(PositionComponent.LevelId), changes, getOldValue, out var levelId)
                         || !matcher.TryGetValue<byte>(
                             entity, (int)EntityComponent.Position, nameof(PositionComponent.LevelX), changes, getOldValue, out var levelX)
                         || !matcher.TryGetValue<byte>(
                             entity, (int)EntityComponent.Position, nameof(PositionComponent.LevelY), changes, getOldValue, out var levelY))
                        {
                            return ((0, 0, 0), false);
                        }

                        return ((levelId, levelX, levelY), true);
                    },
                    new PropertyMatcher()
                        .With(component => ((PositionComponent)component).LevelId, (int)EntityComponent.Position)
                        .With(component => ((PositionComponent)component).LevelX, (int)EntityComponent.Position)
                        .With(component => ((PositionComponent)component).LevelY, (int)EntityComponent.Position)
                ));

            LevelKnowledgesToLevelRelationship = new EntityRelationship<GameEntity>(
                nameof(LevelKnowledgesToLevelRelationship),
                LevelKnowledges,
                Levels,
                new SimpleKeyValueGetter<GameEntity, int>(
                    component => ((PositionComponent)component).LevelId,
                    (int)EntityComponent.Position),
                (effectEntity, _, __) =>
                {
                    effectEntity.RemoveComponent(EntityComponent.Knowledge);
                    effectEntity.RemoveComponent(EntityComponent.Position);
                });

            LevelKnowledgeToLevelEntityRelationship = new UniqueEntityRelationship<GameEntity>(
                nameof(LevelKnowledgeToLevelEntityRelationship),
                LevelKnowledges,
                LevelEntities,
                new SimpleKeyValueGetter<GameEntity, int>(
                    component => ((KnowledgeComponent)component).KnownEntityId,
                    (int)EntityComponent.Knowledge),
                (effectEntity, _, __) =>
                {
                    effectEntity.RemoveComponent(EntityComponent.Knowledge);
                    effectEntity.RemoveComponent(EntityComponent.Position);
                });

            KnowledgeSystem = new KnowledgeSystem();
            queue.Add<VisibleTerrainChangedMessage>(KnowledgeSystem, SensorySystem.VisibleTerrainChangedMessageName, 0);
            queue.Add<TraveledMessage>(KnowledgeSystem, TravelSystem.TraveledMessageName, 4);
            queue.Add<ItemMovedMessage>(KnowledgeSystem, ItemMovingSystem.ItemMovedMessageName, 0);
            queue.Add<ItemEquippedMessage>(KnowledgeSystem, ItemUsageSystem.ItemEquippedMessageName, 0);
            queue.Add<ItemActivatedMessage>(KnowledgeSystem, ItemUsageSystem.ItemActivatedMessageName, 0);
            queue.Add<DiedMessage>(KnowledgeSystem, LivingSystem.DiedMessageName, 1);
            queue.Add<EffectsAppliedMessage>(KnowledgeSystem, EffectApplicationSystem.EffectsAppliedMessageName, 1);
        }
    }
}