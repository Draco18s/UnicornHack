﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UnicornHack.Primitives;
using UnicornHack.Systems.Knowledge;
using UnicornHack.Systems.Levels;

namespace UnicornHack.Hubs
{
    public class LevelItemSnapshot
    {
        private string NameSnapshot { get; set; }

        public LevelItemSnapshot Snapshot(GameEntity itemKnowledgeEntity, SerializationContext context)
        {
            var item = itemKnowledgeEntity.Knowledge.KnownEntity.Item;
            var manager = context.Manager;
            NameSnapshot = context.Services.Language.GetString(item, item.GetQuantity(manager), SenseType.Sight);

            return this;
        }

        public static List<object> Serialize(
            GameEntity knowledgeEntity, EntityState? state, LevelItemSnapshot snapshot, SerializationContext context)
        {
            List<object> properties;
            switch (state)
            {
                case null:
                case EntityState.Added:
                {
                    var manager = context.Manager;
                    var itemKnowledge = knowledgeEntity.Knowledge;
                    var item = itemKnowledge.KnownEntity.Item;
                    var position = knowledgeEntity.Position;
                    properties = state == null
                        ? new List<object>(6)
                        : new List<object>(7) {(int)state};
                    properties.Add(knowledgeEntity.Id);

                    if (itemKnowledge.SensedType.CanIdentify())
                    {
                        properties.Add((int)item.Type);
                        properties.Add(item.TemplateName);
                        properties.Add(
                            context.Services.Language.GetString(item, item.GetQuantity(manager), itemKnowledge.SensedType));
                    }
                    else
                    {
                        properties.Add((int)ItemType.None);
                        properties.Add(null);
                        properties.Add(null);
                    }

                    properties.Add(position.LevelX);
                    properties.Add(position.LevelY);
                    return properties;
                }
                case EntityState.Deleted:
                    return new List<object>
                    {
                        (int)state,
                        knowledgeEntity.Id
                    };
                default:
                {
                    var manager = context.Manager;
                    var itemKnowledge = knowledgeEntity.Knowledge;
                    var item = itemKnowledge.KnownEntity.Item;
                    var position = knowledgeEntity.Position;
                    properties = new List<object>(2)
                    {
                        (int)state,
                        knowledgeEntity.Id
                    };

                    var i = 1;

                    var knowledgeEntry = context.DbContext.Entry(itemKnowledge);
                    var sensedType = knowledgeEntry.Property(nameof(KnowledgeComponent.SensedType));
                    if (sensedType.IsModified)
                    {
                        var canIdentify = itemKnowledge.SensedType.CanIdentify();
                        properties.Add(i);
                        properties.Add(!canIdentify
                            ? (int)ItemType.None
                            : item.Type);

                        i++;
                        properties.Add(i);
                        properties.Add(!canIdentify
                            ? null
                            : item.TemplateName);
                    }
                    else
                    {
                        i++;
                    }

                    i++;
                    var newName = itemKnowledge.SensedType.CanIdentify()
                        ? context.Services.Language.GetString(item, item.GetQuantity(manager), itemKnowledge.SensedType)
                        : null;
                    if (snapshot.NameSnapshot != newName)
                    {
                        properties.Add(i);
                        properties.Add(newName);
                    }

                    var positionEntry = context.DbContext.Entry(position);
                    if (positionEntry.State != EntityState.Unchanged)
                    {
                        i++;
                        var levelX = positionEntry.Property(nameof(PositionComponent.LevelX));
                        if (levelX.IsModified)
                        {
                            properties.Add(i);
                            properties.Add(position.LevelX);
                        }

                        i++;
                        var levelY = positionEntry.Property(nameof(PositionComponent.LevelY));
                        if (levelY.IsModified)
                        {
                            properties.Add(i);
                            properties.Add(position.LevelY);
                        }
                    }

                    return properties.Count > 2 ? properties : null;
                }
            }
        }
    }
}
