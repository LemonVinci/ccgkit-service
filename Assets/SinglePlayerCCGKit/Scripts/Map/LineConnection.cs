// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Represents the connection between two map nodes.
    /// </summary>
    [Serializable]
    public class LineConnection
    {
        public LineRenderer LineRenderer;
        public NodeView From;
        public NodeView To;

        public LineConnection(LineRenderer renderer, NodeView from, NodeView to)
        {
            LineRenderer = renderer;
            From = from;
            To = to;
        }

        public void SetColor(Color color)
        {
            var gradient = LineRenderer.colorGradient;
            var colorKeys = gradient.colorKeys;
            for (var i = 0; i < colorKeys.Length; i++)
                colorKeys[i].color = color;
            gradient.colorKeys = colorKeys;
            LineRenderer.colorGradient = gradient;
        }
    }
}