﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UnicornHack.Systems.Knowledge;

namespace UnicornHack.Hubs
{
    public static class LogEntrySnapshot
    {
        public static List<object> Serialize(LogEntry entry, EntityState? state, SerializationContext context)
        {
            switch (state)
            {
                case null:
                    return new List<object>
                    {
                        entry.Id,
                        ToString(entry)
                    };
                case EntityState.Added:
                    return new List<object>
                    {
                        (int)state,
                        entry.Id,
                        ToString(entry)
                    };
                case EntityState.Deleted:
                    return new List<object>
                    {
                        (int)state,
                        entry.Id
                    };
                default:
                    var properties = new List<object>
                    {
                        (int)state,
                        entry.Id
                    };

                    var logEntry = context.DbContext.Entry(entry);
                    var i = 1;
                    var tick = logEntry.Property(nameof(LogEntry.Tick));
                    var message = logEntry.Property(nameof(LogEntry.Message));
                    if (tick.IsModified
                        || message.IsModified)
                    {
                        properties.Add(i);
                        properties.Add(ToString(entry));
                    }

                    return properties.Count > 2 ? properties : null;
            }
        }

        private static string ToString(LogEntry entry) => $"{entry.Tick / 100f:0000.00}: {entry.Message}";
    }
}
