// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CCGKit
{
    /// <summary>
    /// The logical representation of a map. It consists of a list of nodes and a list
    /// of logical coordinates for those nodes.
    /// </summary>
    [Serializable]
    public class Map
    {
        public List<Node> Nodes;
        public List<Coordinate> Path;

        public Map(List<Node> nodes, List<Coordinate> path)
        {
            Nodes = nodes;
            Path = path;
        }

        public Node GetNode(Coordinate point)
        {
            return Nodes.First(x => x.Coordinate.Equals(point));
        }

        public Node GetBossNode()
        {
            return Nodes.First(x => x.Type == NodeType.Boss);
        }

        public float DistanceBetweenFirstAndLastLayers()
        {
            var bossNode = GetBossNode();
            var firstLayerNode = Nodes.First(x => x.Coordinate.Y == 0);

            if (bossNode == null || firstLayerNode == null)
                return 0f;

            return bossNode.Position.y - firstLayerNode.Position.y;
        }
    }
}