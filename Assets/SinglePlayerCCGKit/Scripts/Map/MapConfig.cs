// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The configuration of a map, which is a scriptable-object asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Single-Player CCG Kit/Map/Config", fileName = "MapConfig", order = 0)]
    public class MapConfig : ScriptableObject
    {
        public List<Layer> Layers;
        public List<NodeConfig> Nodes;

        public int GridWidth => Mathf.Max(NumStartingNodes.Max, NumPreBossNodes.Max);

        public IntMinMax NumStartingNodes;
        public IntMinMax NumPreBossNodes;
    }
}