using System;
using System.Collections.Generic;
using System.Linq;
using CSharpScriptSerialization;
using UnicornHack.Data.Fragments;
using UnicornHack.Primitives;
using UnicornHack.Systems.Levels;
using UnicornHack.Utils.DataLoading;
using UnicornHack.Utils.DataStructures;

namespace UnicornHack.Generation.Map
{
    public class ConnectingMapFragment : MapFragment
    {
        public ICollection<LevelConnection> Connections { get; set; } = new HashSet<LevelConnection>();

        private Func<string, byte, int, int, ConnectionComponent, float> _weightFunction;

        public float GetWeight(LevelComponent level, Rectangle boundingRectangle, ConnectionComponent target)
        {
            // TODO: take transformations into account
            if (PayloadArea.Width > boundingRectangle.Width || PayloadArea.Height > boundingRectangle.Height)
            {
                return 0;
            }

            if (_weightFunction == null)
            {
                _weightFunction = (GenerationWeight ?? new DefaultWeight()).CreateConnectingFragmentWeightFunction();
            }

            return _weightFunction(level.Branch.Name, level.Depth, 0, 0, target);
        }

        public override Room BuildRoom(LevelComponent level, IEnumerable<Point> points, Action<Point> insideAction,
            Action<Point> perimeterAction, Action<Point> outsideAction)
        {
            var room = base.BuildRoom(level, points, insideAction, perimeterAction, outsideAction);
            if (room == null
                || room.InsidePoints.Count == 0)
            {
                return room;
            }

            var connectionDefinitions = Connections;
            if (connectionDefinitions.Count == 0)
            {
                connectionDefinitions = new[] {new LevelConnection()};
            }

            var manager = level.Entity.Manager;
            foreach (var levelConnection in connectionDefinitions.Where(c => c.Glyph == null))
            {
                var connectionPoint = level.GenerationRandom.Pick(room.InsidePoints,
                    p => manager.ConnectionsToLevelRelationship[level.EntityId]
                        .All(c => c.Position.LevelCell != p));
                CreateConnection(level, connectionPoint, levelConnection);
            }

            return room;
        }

        protected override void Write(char c, Point point, LevelComponent level,
            (List<Point> doorwayPoints, List<Point> perimeterPoints, List<Point> insidePoints, List<Point> points)
                state)
        {
            MapFeature feature;
            switch (c)
            {
                case '<':
                case '{':
                case '[':
                case '>':
                case '}':
                case ']':
                    feature = MapFeature.StoneFloor;
                    state.points.Add(point);
                    CreateConnection(level, point, c);
                    break;
                default:
                    base.Write(c, point, level, state);
                    return;
            }

            level.Terrain[level.PointToIndex[point.X, point.Y]] = (byte)feature;
        }

        protected virtual void CreateConnection(LevelComponent level, Point point, char? glyph)
            => CreateConnection(level, point, Connections.FirstOrDefault(c => c.Glyph == glyph));

        protected virtual ConnectionComponent CreateConnection(LevelComponent level, Point point,
            LevelConnection connectionDefinition)
        {
            var manager = level.Entity.Manager;

            foreach (var connectionEntity in manager.IncomingConnectionsToLevelRelationship[level.EntityId])
            {
                var connection = connectionEntity.Connection;
                if (connection.TargetLevelX == null)
                {
                    var target = manager.FindEntity(connection.TargetLevelId).Level;
                    if ((connectionDefinition?.BranchName == null ||
                         connectionDefinition.BranchName == target.BranchName)
                        && (connectionDefinition?.Depth == null ||
                            connectionDefinition.Depth == target.Depth))
                    {
                        return LevelConnection.CreateReceivingConnection(level, point, connection);
                    }
                }
            }

            return connectionDefinition?.BranchName != null
                ? LevelConnection.CreateSourceConnection(level, point, connectionDefinition.BranchName,
                    connectionDefinition.Depth ?? 1)
                : LevelConnection.CreateSourceConnection(level, point, level.BranchName,
                    connectionDefinition?.Depth ?? (byte)(level.Depth + 1));
        }

        public static readonly CSScriptLoader<ConnectingMapFragment> Loader =
            new CSScriptLoader<ConnectingMapFragment>(@"Data\Fragments\Connecting\", typeof(ConnectingMapFragmentData));

        private static readonly CSScriptSerializer Serializer =
            new PropertyCSScriptSerializer<ConnectingMapFragment>(GetPropertyConditions<ConnectingMapFragment>());

        protected static new Dictionary<string, Func<TConnectingMapFragment, object, bool>>
            GetPropertyConditions<TConnectingMapFragment>() where TConnectingMapFragment : ConnectingMapFragment
        {
            var propertyConditions = MapFragment.GetPropertyConditions<TConnectingMapFragment>();
            var mapCondition = propertyConditions[nameof(Map)];
            propertyConditions.Remove(nameof(Map));

            propertyConditions.Add(nameof(Connections),
                (o, v) => v != null && ((ICollection<LevelConnection>)v).Count != 0);
            propertyConditions.Add(nameof(Map), mapCondition);
            return propertyConditions;
        }

        public override ICSScriptSerializer GetSerializer() => Serializer;
    }
}
