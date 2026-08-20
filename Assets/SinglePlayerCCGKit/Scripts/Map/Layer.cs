// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// A map is divided into different levels/heights, which we refer to as layers. A layer is
    /// a scriptable object asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Single-Player CCG Kit/Map/Layer", fileName = "Layer", order = 1)]
    public class Layer : ScriptableObject
    {
        public NodeType Type;
        public FloatMinMax DistanceFromPreviousLayer;
        public int DistanceBetweenNodes;

        [Range(0f, 1f)]
        public float RandomizePosition;

        [Range(0f, 1f)]
        public float EnemyNodeChance;
        [Range(0f, 1f)]
        public float EliteNodeChance;
        [Range(0f, 1f)]
        public float RestNodeChance;
        [Range(0f, 1f)]
        public float TreasureNodeChance;
        [Range(0f, 1f)]
        public float MerchantNodeChance;
        [Range(0f, 1f)]
        public float UnknownNodeChance;
        [Range(0f, 1f)]
        public float BossNodeChance;
    }
}