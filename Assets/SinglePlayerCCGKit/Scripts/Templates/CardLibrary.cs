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
        menuName = "Single-Player CCG Kit/Templates/Card library",
        fileName = "CardLibrary",
        order = 3)]
    public class CardLibrary : ScriptableObject
    {
        public string Name;
        public List<CardLibraryEntry> Entries = new List<CardLibraryEntry>();
    }
}
