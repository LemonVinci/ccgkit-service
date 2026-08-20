// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// World-space target for the targeting arrow. Each instance is one dish station the player can add ingredients to.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class DishTarget : MonoBehaviour
    {
        public int StationIndex { get; private set; }

        public void Configure(int stationIndex)
        {
            StationIndex = stationIndex;
            gameObject.name = "DishTarget_" + stationIndex;
        }
    }
}
