﻿using UnicornHack.Systems.Abilities;
using UnicornHack.Systems.Actors;
using UnicornHack.Systems.Items;
using UnicornHack.Systems.Levels;
using UnicornHack.Utils.MessagingECS;

// ReSharper disable once CheckNamespace
namespace UnicornHack
{
    public partial class GameManager
    {
        public EntityGroup<GameEntity> Players { get; private set; }
        public EntityGroup<GameEntity> LevelActors { get; private set; }
        public UniqueEntityIndex<GameEntity, (int, byte, byte)> LevelActorToLevelCellIndex { get; private set; }
        public EntityRelationship<GameEntity> LevelActorsToLevelRelationship { get; private set; }
        public PlayerSystem PlayerSystem { get; private set; }
        public AISystem AISystem { get; private set; }

        private void InitializeActors(SequentialMessageQueue<GameManager> queue)
        {
            Add<PlayerComponent>(EntityComponent.Player, poolSize: 1);
            Add<AIComponent>(EntityComponent.AI, poolSize: 32);

            Players = CreateGroup(nameof(Players),
                new EntityMatcher<GameEntity>().AllOf((int)EntityComponent.Player));

            LevelActors = CreateGroup(nameof(LevelActors),
                new EntityMatcher<GameEntity>().AllOf((int)EntityComponent.Position)
                    .AnyOf((int)EntityComponent.AI, (int)EntityComponent.Player));

            LevelActorToLevelCellIndex = new UniqueEntityIndex<GameEntity, (int, byte, byte)>(
                LevelActors,
                new KeyValueGetter<GameEntity, (int, byte, byte)>(
                    (entity, changedComponentId, changedComponent, changedProperty, changedValue) =>
                    {
                        PositionComponent position;
                        if (changedComponentId == (int)EntityComponent.Position)
                        {
                            position = (PositionComponent)changedComponent;
                        }
                        else
                        {
                            position = entity.Position;
                        }

                        var levelId = position.LevelId;
                        var levelX = position.LevelX;
                        var levelY = position.LevelY;

                        switch (changedProperty)
                        {
                            case nameof(PositionComponent.LevelId):
                                levelId = (int)changedValue;
                                break;
                            case nameof(PositionComponent.LevelX):
                                levelX = (byte)changedValue;
                                break;
                            case nameof(PositionComponent.LevelY):
                                levelY = (byte)changedValue;
                                break;
                        }

                        return ((levelId, levelX, levelY), true);
                    },
                    new PropertyMatcher((int)EntityComponent.Position, nameof(PositionComponent.LevelId))
                        .With((int)EntityComponent.Position, nameof(PositionComponent.LevelX))
                        .With((int)EntityComponent.Position, nameof(PositionComponent.LevelY))
                ));

            LevelActorsToLevelRelationship = new EntityRelationship<GameEntity>(
                nameof(LevelActorsToLevelRelationship),
                LevelActors,
                Levels,
                new SimpleNonNullableKeyValueGetter<GameEntity, int>(
                    component => ((PositionComponent)component).LevelId,
                    (int)EntityComponent.Position),
                (effectEntity, _, __, ___) => effectEntity.RemoveComponent(EntityComponent.Position));

            AISystem = new AISystem();
            queue.Add<PerformActionMessage>(AISystem, AISystem.PerformAIActionMessageName, 0);
            queue.Add<AbilityActivatedMessage>(AISystem, AbilityActivationSystem.AbilityActivatedMessageName, 1);
            queue.Add<TraveledMessage>(AISystem, TravelSystem.TraveledMessageName, 1);
            queue.Add<ItemMovedMessage>(AISystem, ItemMovingSystem.ItemMovedMessageName, 1);
            queue.Add<ItemEquippedMessage>(AISystem, ItemUsageSystem.ItemEquippedMessageName, 1);
            queue.Add<ItemActivatedMessage>(AISystem, ItemUsageSystem.ItemActivatedMessageName, 1);

            PlayerSystem = new PlayerSystem();
            queue.Add<PerformActionMessage>(PlayerSystem, PlayerSystem.PerformPlayerActionMessageName, 0);
            queue.Add<AbilityActivatedMessage>(PlayerSystem, AbilityActivationSystem.AbilityActivatedMessageName, 2);
            queue.Add<TraveledMessage>(PlayerSystem, TravelSystem.TraveledMessageName, 2);
            queue.Add<ItemEquippedMessage>(PlayerSystem, ItemUsageSystem.ItemEquippedMessageName, 2);
            queue.Add<ItemActivatedMessage>(PlayerSystem, ItemUsageSystem.ItemActivatedMessageName, 2);
            queue.Add<ItemMovedMessage>(PlayerSystem, ItemMovingSystem.ItemMovedMessageName, 2);
        }
    }
}
