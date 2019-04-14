﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UnicornHack.Primitives;
using UnicornHack.Systems.Knowledge;
using UnicornHack.Systems.Levels;

namespace UnicornHack.Hubs
{
    public static class LevelActorSnapshot
    {
        public static List<object> Serialize(
            GameEntity knowledgeEntity, EntityState? state, SerializationContext context)
        {
            List<object> properties;
            switch (state)
            {
                case null:
                case EntityState.Added:
                {
                    var actorKnowledge = knowledgeEntity.Knowledge;
                    var knownEntity = actorKnowledge.KnownEntity;
                    var ai = knownEntity.AI;
                    var position = knowledgeEntity.Position;
                    var manager = context.Manager;
                    properties = state == null
                        ? new List<object>(6)
                        : new List<object>(7) {(int)state};
                    properties.Add(actorKnowledge.EntityId);
                    // TODO: Move to language service
                    if (actorKnowledge.SensedType.CanIdentify())
                    {
                        properties.Add(ai != null
                            ? manager.RacesToBeingRelationship[actorKnowledge.KnownEntityId].Values.First().Race
                                .TemplateName
                            : "player");
                        properties.Add(context.Services.Language.GetActorName(knownEntity, actorKnowledge.SensedType));
                    }
                    else
                    {
                        properties.Add(null);
                        properties.Add(null);
                    }

                    properties.Add(position.LevelX);
                    properties.Add(position.LevelY);
                    properties.Add((byte)position.Heading);
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
                    var actorKnowledge = knowledgeEntity.Knowledge;
                    var knownEntity = actorKnowledge.KnownEntity;
                    var ai = knownEntity.AI;
                    var position = knowledgeEntity.Position;
                    var manager = context.Manager;
                    properties = new List<object>(2)
                    {
                        (int)state,
                        actorKnowledge.EntityId
                    };

                    var knowledgeEntry = context.DbContext.Entry(actorKnowledge);
                    var i = 1;
                    var sensedType = knowledgeEntry.Property(nameof(KnowledgeComponent.SensedType));
                    if (sensedType.IsModified)
                    {
                        var canIdentify = actorKnowledge.SensedType.CanIdentify();
                        properties.Add(i);
                        properties.Add(!canIdentify
                            ? null
                            : ai != null
                                ? manager.RacesToBeingRelationship[actorKnowledge.KnownEntityId].Values.First().Race
                                    .TemplateName
                                : "player");

                        i++;
                        properties.Add(i);
                        properties.Add(!canIdentify
                            ? null
                            : context.Services.Language.GetActorName(knownEntity, actorKnowledge.SensedType));
                    }
                    else
                    {
                        i++;
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

                        i++;
                        var heading = positionEntry.Property(nameof(PositionComponent.Heading));
                        if (heading.IsModified)
                        {
                            properties.Add(i);
                            properties.Add((byte)position.Heading);
                        }
                    }

                    return properties.Count > 2 ? properties : null;
                }
            }
        }

        public static List<object> SerializeAttributes(GameEntity actorEntity, SenseType sense, SerializationContext context)
        {
            var canIdentify = actorEntity != null
                                  && (sense & (SenseType.Sight | SenseType.Touch)) != SenseType.None;
            if (!canIdentify)
            {
                return new List<object>();
            }

            var being = actorEntity.Being;
            var sensor = actorEntity.Sensor;
            return new List<object>(37)
            {
                context.Services.Language.GetActorName(actorEntity, sense),
                context.Services.Language.GetActorDescription(actorEntity),
                actorEntity.Position.MovementDelay,
                sensor.PrimaryFOVQuadrants,
                sensor.PrimaryVisionRange,
                sensor.TotalFOVQuadrants,
                sensor.SecondaryVisionRange,
                sensor.Infravision,
                sensor.InvisibilityDetection,
                being.Visibility,
                being.HitPoints,
                being.HitPointMaximum,
                being.EnergyPoints,
                being.EnergyPointMaximum,
                being.Might,
                being.Speed,
                being.Focus,
                being.Perception,
                being.Regeneration,
                being.EnergyRegeneration,
                being.Armor,
                being.Deflection,
                being.Evasion,
                being.PhysicalResistance,
                being.MagicResistance,
                being.BleedingResistance,
                being.AcidResistance,
                being.ColdResistance,
                being.ElectricityResistance,
                being.FireResistance,
                being.PsychicResistance,
                being.ToxinResistance,
                being.VoidResistance,
                being.SonicResistance,
                being.StunResistance,
                being.LightResistance,
                being.WaterResistance
            };
        }
    }
}
