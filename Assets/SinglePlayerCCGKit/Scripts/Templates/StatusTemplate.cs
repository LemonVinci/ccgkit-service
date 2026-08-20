// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Templates/Status",
        fileName = "Status",
        order = 5)]
    public class StatusTemplate : ScriptableObject
    {
        public string Name;
        public Sprite Sprite;
    }
}