// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;

namespace CCGKit
{
    /// <summary>
    /// Utility type that computes a random floating-point value in the given range.
    /// </summary>
    [Serializable]
    public class FloatMinMax
    {
        public float Min;
        public float Max;

        public float GetValue(Random rng)
        {
            return rng.NextFloatInRange(Min, Max);
        }
    }
}