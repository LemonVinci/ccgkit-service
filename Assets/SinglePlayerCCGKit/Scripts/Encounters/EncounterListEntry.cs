// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEngine;

namespace CCGKit
{
    [Serializable]
    public class EncounterListEntry
    {
        public string Name;
        public Sprite Background;
        [Tooltip("Dish the player must complete by playing ingredients to match all requirement lines.")]
        public Dish Dish;
    }
}
