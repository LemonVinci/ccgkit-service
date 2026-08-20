// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The configuration of an encounter list, which is a scriptable-object asset.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "Single-Player CCG Kit/Encounters/Encounter list", fileName = "EncounterList", order = 3)]
    public class EncounterList : ScriptableObject
    {
        public List<EncounterListEntry> Encounters = new List<EncounterListEntry>();
    }
}