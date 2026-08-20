// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;

namespace CCGKit
{
    /// <summary>
    /// Utility type that computes a random integer value in the given range.
    /// </summary>
    [Serializable]
    public class IntMinMax
    {
        public int Min;
        public int Max;

        public int GetValue(Random rng)
        {
            return rng.Next(Min, Max+1);
        }
    }
}