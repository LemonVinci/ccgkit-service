// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;

namespace CCGKit
{
    /// <summary>
    /// Miscellaneous random-generation utility methods.
    /// </summary>
    public static class RandomExtensionMethods
    {
        public static float NextFloatInRange(this Random rng, float min, float max)
        {
            return (float)(rng.NextDouble() * (max - min) + min);
        }
    }
}