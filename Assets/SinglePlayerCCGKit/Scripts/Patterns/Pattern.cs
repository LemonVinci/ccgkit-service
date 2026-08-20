// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    public abstract class Pattern : ScriptableObject
    {
        public List<Effect> Effects = new List<Effect>();

        public abstract string GetName();

#if UNITY_EDITOR
        public abstract void Draw();
#endif
    }
}
