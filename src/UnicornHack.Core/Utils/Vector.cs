using System;

namespace UnicornHack.Utils
{
    public struct Vector
    {
        public Vector(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        public sbyte X { get; }
        public sbyte Y { get; }

        public static Vector Convert(Direction direction)
        {
            switch (direction)
            {
                case Direction.None:
                    return new Vector(x: 0, y: 0);
                case Direction.North:
                    return new Vector(x: 0, y: -1);
                case Direction.South:
                    return new Vector(x: 0, y: 1);
                case Direction.West:
                    return new Vector(x: -1, y: 0);
                case Direction.East:
                    return new Vector(x: 1, y: 0);
                case Direction.Northwest:
                    return new Vector(x: -1, y: -1);
                case Direction.Northeast:
                    return new Vector(x: 1, y: -1);
                case Direction.Southwest:
                    return new Vector(x: -1, y: 1);
                case Direction.Southeast:
                    return new Vector(x: 1, y: 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, message: null);
            }
        }
    }
}