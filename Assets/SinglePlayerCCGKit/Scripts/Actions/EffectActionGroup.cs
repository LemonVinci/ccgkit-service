// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Templates/Action group",
        fileName = "Action group",
        order = 6)]
    public class EffectActionGroup : ScriptableObject
    {
        public List<EffectAction> Actions = new List<EffectAction>();
    }
}