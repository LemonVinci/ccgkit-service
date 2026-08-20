// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;

namespace CCGKit
{
    /// <summary>
    /// Stores the logical X and Y coordinates of a map node (as opposed to the actual floating-point
    /// world position).
    /// </summary>
    [Serializable]
    public struct Coordinate : IEquatable<Coordinate>
    {
        public int X;
        public int Y;

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj) => Equals((Coordinate)obj);
        public bool Equals(Coordinate other) => (X, Y) == (other.X, other.Y);

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static bool operator==(Coordinate a, Coordinate b) => Equals(a, b);
        public static bool operator!=(Coordinate a, Coordinate b) => !Equals(a, b);
    }
}