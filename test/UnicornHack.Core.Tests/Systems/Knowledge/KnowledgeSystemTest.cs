﻿using System.Linq;
using UnicornHack.Data.Creatures;
using UnicornHack.Data.Items;
using UnicornHack.Generation;
using UnicornHack.Primitives;
using UnicornHack.Utils.DataStructures;
using Xunit;

namespace UnicornHack.Systems.Knowledge
{
    public class KnowledgeSystemTest
    {
        private readonly static string TestMap = @"
...
>..
..#";

        [Fact]
        public void Knowledge_updated_on_travel()
        {
            var level = TestHelper.BuildLevel(TestMap);
            ItemData.Dagger.Instantiate(level, new Point(0, 0));
            var nymph = CreatureData.WaterNymph.Instantiate(level, new Point(0, 0));
            var player = PlayerRace.InstantiatePlayer("Dudley", Sex.Male, level.Entity, new Point(0, 2));
            player.Position.Heading = Direction.West;
            var manager = player.Manager;

            manager.Queue.ProcessQueue(manager);

            var nymphKnowledge = manager.LevelKnowledgeToLevelEntityRelationship[nymph.Id];
            var playerKnowledge = manager.LevelKnowledgeToLevelEntityRelationship[player.Id];
            var dagger = manager.LevelItemsToLevelCellIndex[(level.EntityId, 0, 0)];
            var daggerKnowledge = manager.LevelKnowledgeToLevelEntityRelationship[dagger.Id];
            var connection = manager.ConnectionsToLevelRelationship[level.EntityId].Single();
            var connectionKnowledge = manager.LevelKnowledgeToLevelEntityRelationship[connection.Id];

            Assert.Equal(4, manager.LevelKnowledges.Count);
            Assert.Equal(4, manager.LevelKnowledgesToLevelRelationship[level.EntityId].Count);

            Assert.Equal(nymph.Position.LevelCell, nymphKnowledge.Position.LevelCell);
            Assert.Equal(nymph.Position.Heading, nymphKnowledge.Position.Heading);
            Assert.Same(nymph, nymphKnowledge.Knowledge.KnownEntity);
            Assert.Equal(SenseType.Sight, nymphKnowledge.Knowledge.SensedType);
            Assert.Same(manager.LevelKnowledgeToLevelCellIndex[(level.EntityId, 0, 0)].Last(), nymphKnowledge);

            Assert.Equal(player.Position.LevelCell, playerKnowledge.Position.LevelCell);
            Assert.Equal(player.Position.Heading, playerKnowledge.Position.Heading);
            Assert.Same(player, playerKnowledge.Knowledge.KnownEntity);
            Assert.Equal(SenseType.Sight | SenseType.Touch | SenseType.Telepathy, playerKnowledge.Knowledge.SensedType);
            Assert.Same(manager.LevelKnowledgeToLevelCellIndex[(level.EntityId, 0, 2)].Single(), playerKnowledge);

            Assert.Equal(dagger.Position.LevelCell, daggerKnowledge.Position.LevelCell);
            Assert.Equal(dagger.Position.Heading, daggerKnowledge.Position.Heading);
            Assert.Same(dagger, daggerKnowledge.Knowledge.KnownEntity);
            Assert.Equal(SenseType.Sight, daggerKnowledge.Knowledge.SensedType);
            Assert.Same(manager.LevelKnowledgeToLevelCellIndex[(level.EntityId, 0, 0)].First(), daggerKnowledge);

            Assert.Equal(connection.Position.LevelCell, connectionKnowledge.Position.LevelCell);
            Assert.Equal(connection.Position.Heading, connectionKnowledge.Position.Heading);
            Assert.Same(connection, connectionKnowledge.Knowledge.KnownEntity);
            Assert.Equal(SenseType.Sight, connectionKnowledge.Knowledge.SensedType);
            Assert.Same(manager.LevelKnowledgeToLevelCellIndex[(level.EntityId, 0, 1)].Single(), connectionKnowledge);

            var travelMessage = manager.TravelSystem.CreateTravelMessage(manager);
            travelMessage.Entity = nymph;
            travelMessage.TargetHeading = Direction.East;
            travelMessage.TargetCell = new Point(1, 1);
            manager.Enqueue(travelMessage);

            var moveMessage = manager.ItemMovingSystem.CreateMoveItemMessage(manager);
            moveMessage.ItemEntity = dagger;
            moveMessage.TargetLevelEntity = level.Entity;
            moveMessage.TargetCell = new Point(1, 0);
            manager.Enqueue(moveMessage);

            manager.Queue.ProcessQueue(manager);

            Assert.Equal(4, manager.LevelKnowledges.Count);
            Assert.Equal(4, manager.LevelKnowledgesToLevelRelationship[level.EntityId].Count);
            Assert.Same(nymphKnowledge, manager.LevelKnowledgeToLevelEntityRelationship[nymph.Id]);
            Assert.Same(daggerKnowledge, manager.LevelKnowledgeToLevelEntityRelationship[dagger.Id]);

            Assert.Equal(nymph.Position.LevelCell, nymphKnowledge.Position.LevelCell);
            Assert.Equal(nymph.Position.Heading, nymphKnowledge.Position.Heading);
            Assert.Same(nymph, nymphKnowledge.Knowledge.KnownEntity);
            Assert.Equal(SenseType.Sight, nymphKnowledge.Knowledge.SensedType);
            Assert.Same(manager.LevelKnowledgeToLevelCellIndex[(level.EntityId, 1, 1)].Last(), nymphKnowledge);

            Assert.Equal(dagger.Position.LevelCell, daggerKnowledge.Position.LevelCell);
            Assert.Equal(dagger.Position.Heading, daggerKnowledge.Position.Heading);
            Assert.Same(dagger, daggerKnowledge.Knowledge.KnownEntity);
            Assert.Equal(SenseType.Sight, daggerKnowledge.Knowledge.SensedType);
            Assert.Same(manager.LevelKnowledgeToLevelCellIndex[(level.EntityId, 1, 0)].First(), daggerKnowledge);
        }
    }
}
