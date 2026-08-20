// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [Serializable]
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Templates/Enemy",
        fileName = "Enemy",
        order = 2)]
    public class EnemyTemplate : CharacterTemplate
    {
        public int HpLow;
        public int HpHigh;

        public List<Pattern> Patterns = new List<Pattern>();
    }
}
