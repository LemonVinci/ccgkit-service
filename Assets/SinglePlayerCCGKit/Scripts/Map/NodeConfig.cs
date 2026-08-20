// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The configuration of a map node, which is a scriptable-object asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Single-Player CCG Kit/Map/Node", fileName = "NodeConfig", order = 2)]
    public class NodeConfig : ScriptableObject
    {
        public Sprite Sprite;
        public NodeType Type;
    }
}