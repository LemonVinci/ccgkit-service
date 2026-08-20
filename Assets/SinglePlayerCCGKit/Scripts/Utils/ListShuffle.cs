// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// Utility extension method to shuffle NativeLists.
    /// </summary>
    public static class ListShuffle
    {
        private static readonly Random Rng = new Random();

        public static void Shuffle<T>(this List<T> list)
        {
            var n = list.Count;
            while (n --> 1)
            {
                var k = Rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this List<T> list, Random rng)
        {
            var n = list.Count;
            while (n --> 1)
            {
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
