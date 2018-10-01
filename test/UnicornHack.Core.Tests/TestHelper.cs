﻿using System;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using UnicornHack.Generation;
using UnicornHack.Generation.Map;
using UnicornHack.Primitives;
using UnicornHack.Services;
using UnicornHack.Services.English;
using UnicornHack.Systems.Levels;
using UnicornHack.Utils;
using UnicornHack.Utils.DataStructures;
using UnicornHack.Utils.MessagingECS;
using Xunit;

namespace UnicornHack
{
    public static class TestHelper
    {
        public static Game CreateGame(uint? seed = null)
        {
            var game = new Game
            {
                Random = new SimpleRandom {Seed = seed ?? (uint)Environment.TickCount},
                InitialSeed = seed,
                Repository = new TestRepository(),
                Services = new GameServices(new EnglishLanguageService(), new MemoryCache(new MemoryCacheOptions()))
            };

            game.Manager = CreateGameManager(game);

            return game;
        }

        public static GameManager CreateGameManager(Game game = null)
        {
            var manager = new GameManager {Game = game ?? CreateGame()};
            manager.Initialize(new SequentialMessageQueue<GameManager>());
            return manager;
        }

        public static LevelComponent BuildLevel(string map = "", uint? seed = null)
        {
            var game = CreateGame(seed);

            var fragment = new DefiningMapFragment
            {
                Map = map,
                Layout = new EmptyLayout {Coverage = 0},
                CreatureGenerator = new CreatureGenerator {ExpectedInitialCount = 0},
                ItemGenerator = new ItemGenerator {ExpectedInitialCount = 0}
            };

            return CreateLevel(fragment, game);
        }

        public static LevelComponent CreateLevel(DefiningMapFragment fragment, Game game)
        {
            var branch = new GameBranch
            {
                Game = game,
                Name = "test branch",
                Length = 1,
                Difficulty = 1
            };
            game.Branches.Add(branch);

            fragment.EnsureInitialized(game);
            if (fragment.Height != 0)
            {
                fragment.LevelHeight = fragment.Height;
            }

            if (fragment.Width != 0)
            {
                fragment.LevelWidth = fragment.Width;
            }

            var level = LevelGenerator.CreateEmpty(branch, 0, game.Random.Seed, game.Manager);
            LevelGenerator.Generate(level, fragment);

            return level;
        }

        public static void AssertVisibility(LevelComponent level, string expectedVisbileMap, byte[] actualVisibility)
        {
            var expectedFragment =
                new NormalMapFragment
                {
                    Map = expectedVisbileMap,
                    Width = level.Width,
                    Height = level.Height
                };
            expectedFragment.EnsureInitialized(level.Game);

            var expected = new byte[level.Height * level.Width];
            var matched = true;

            expectedFragment.WriteMap(
                new Point(0, 0),
                level,
                (c, point, l, _) =>
                {
                    var expectedVisible = c == ' ' ? (byte)0 : (byte)1;
                    var i = l.PointToIndex[point.X, point.Y];
                    expected[i] = expectedVisible;
                    var actualVisibile = actualVisibility[i] == 0 ? 0 : 1;
                    matched &= expectedVisible == actualVisibile;
                },
                (object)null);

            Assert.True(matched, @"Expected:
" + PrintMap(level, expected) + @"
Actual:
" + PrintMap(level, actualVisibility) + @"
Seed: " + level.Game.InitialSeed);
        }

        public static void AssertTerrain(LevelComponent level, string expectedMap, byte[] actualTerrain)
        {
            var expectedFragment =
                new NormalMapFragment
                {
                    Map = expectedMap,
                    Width = level.Width,
                    Height = level.Height
                };
            expectedFragment.EnsureInitialized(level.Game);

            var expected = new byte[level.Height * level.Width];
            var matched = true;

            expectedFragment.WriteMap(
                new Point(0, 0),
                level,
                (c, point, l, _) =>
                {
                    var expectedFeature = (byte)ToMapFeature(c);
                    var i = l.PointToIndex[point.X, point.Y];
                    expected[i] = expectedFeature;
                    if (expectedFeature != actualTerrain[i]
                        && (expectedFeature == (byte)MapFeature.Default
                            || expectedFeature == (byte)MapFeature.Unexplored)
                        && !(actualTerrain[i] == (byte)MapFeature.Default
                            || actualTerrain[i] == (byte)MapFeature.Unexplored))
                    {
                        matched = false;
                    }
                },
                (object)null);

            Assert.True(matched, @"Expected:
" + PrintMap(level, null, expected) + @"
Actual:
" + PrintMap(level, null, actualTerrain) + @"
Seed: " + level.Game.InitialSeed);
        }

        public static MapFeature ToMapFeature(char c)
        {
            var feature = MapFeature.Default;
            switch (c)
            {
                case '.':
                    feature = MapFeature.StoneFloor;
                    goto case '\u0001';
                case ',':
                    feature = MapFeature.RockFloor;
                    goto case '\u0001';
                case '?':
                    feature = MapFeature.StoneFloor;
                    goto case '\u0001';
                case '#':
                    feature = MapFeature.StoneWall;
                    goto case '\u0001';
                case 'A':
                    feature = MapFeature.StoneArchway;
                    goto case '\u0001';
                case '=':
                    feature = MapFeature.Pool;
                    goto case '\u0001';
                case '\u0001':
                    break;
                case ' ':
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported map character '{c}'");
            }

            return feature;
        }

        public static string PrintMap(LevelComponent level, byte[] visibleTerrain, byte[] terrain = null)
        {
            terrain = terrain ?? level.Terrain;
            var builder = new StringBuilder();
            var i = 0;
            for (var y = 0; y < level.Height; y++)
            {
                for (var x = 0; x < level.Width; x++)
                {
                    if (visibleTerrain != null && visibleTerrain[i] == 0)
                    {
                        builder.Append(' ');
                    }
                    else
                    {
                        var feature = (MapFeature)terrain[i];
                        var symbol = ' ';
                        switch (feature)
                        {
                            case MapFeature.Default:
                            case MapFeature.Unexplored:
                                break;
                            case MapFeature.StoneFloor:
                                symbol = '·';
                                break;
                            case MapFeature.RockFloor:
                                symbol = '█';
                                break;
                            case MapFeature.StoneWall:
                                var neighbours = (DirectionFlags)level.WallNeighbours[i] & DirectionFlags.Cross;
                                switch (neighbours)
                                {
                                    case DirectionFlags.None:
                                        symbol = '●';
                                        break;
                                    case DirectionFlags.North:
                                        symbol = '╹';
                                        break;
                                    case DirectionFlags.East:
                                        symbol = '╺';
                                        break;
                                    case DirectionFlags.NorthAndEast:
                                        symbol = '┗';
                                        break;
                                    case DirectionFlags.South:
                                        symbol = '╻';
                                        break;
                                    case DirectionFlags.Longitudinal:
                                        symbol = '┃';
                                        break;
                                    case DirectionFlags.SouthAndEast:
                                        symbol = '┏';
                                        break;
                                    case DirectionFlags.NorthEastSouth:
                                        symbol = '┣';
                                        break;
                                    case DirectionFlags.West:
                                        symbol = '╸';
                                        break;
                                    case DirectionFlags.NorthAndWest:
                                        symbol = '┛';
                                        break;
                                    case DirectionFlags.Latitudinal:
                                        symbol = '━';
                                        break;
                                    case DirectionFlags.NorthEastWest:
                                        symbol = '┻';
                                        break;
                                    case DirectionFlags.SouthAndWest:
                                        symbol = '┓';
                                        break;
                                    case DirectionFlags.NorthWestSouth:
                                        symbol = '┫';
                                        break;
                                    case DirectionFlags.SouthEastWest:
                                        symbol = '┳';
                                        break;
                                    case DirectionFlags.Cross:
                                        symbol = '╋';
                                        break;
                                    default:
                                        throw new InvalidOperationException("Invalid wall neighbours: " + neighbours);
                                }

                                break;
                            case MapFeature.StoneArchway:
                                symbol = '∩';
                                break;
                            case MapFeature.Pool:
                                symbol = '≈';
                                break;
                            default:
                                throw new NotSupportedException($"Map feature {feature} not supported.");
                        }

                        builder.Append(symbol);
                    }

                    i++;
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
