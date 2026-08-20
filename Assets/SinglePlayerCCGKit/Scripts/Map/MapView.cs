// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This component is responsible for rendering the given map on the screen.
    /// </summary>
    public class MapView : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private MapConfig config;

        [SerializeField]
        private GameObject nodePrefab;

        [SerializeField]
        private float orientationOffset;

        [Header("Lines")]
        [SerializeField]
        private GameObject linePrefab;
        [SerializeField]
        private float offsetFromNodes = 0.5f;
        [SerializeField]
        private Color lineLockedColor;
        [SerializeField]
        private Color lineVisitedColor;

        [Header("Background")]
        [SerializeField]
        private Sprite background;
        [SerializeField]
        private float xSize;
        [SerializeField]
        private float yOffset;
#pragma warning restore 649

        public readonly List<NodeView> MapNodes = new List<NodeView>();
        private readonly List<LineConnection> lineConnections = new List<LineConnection>();

        private GameObject firstParent;
        private GameObject mapParent;

        private Map map;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public void ShowMap(Map map)
        {
            this.map = map;
            CreateMapParent();
            CreateNodes(map.Nodes);
            DrawLines();
            SetOrientation();
            SetReachableNodes();
            SetLineColors();
            CreateBackground();
        }

        private void CreateMapParent()
        {
            firstParent = new GameObject("Map");
            mapParent = new GameObject("Scroll");
            mapParent.transform.SetParent(firstParent.transform);

            var scroll = mapParent.AddComponent<ScrollNonUI>();
            scroll.FreezeX = true;
            scroll.FreezeY = false;

            var collider = mapParent.AddComponent<BoxCollider>();
            collider.size = new Vector3(100, 100, 1);
        }

        private void CreateNodes(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var mapNode = CreateMapNode(node);
                MapNodes.Add(mapNode);
            }
        }

        private NodeView CreateMapNode(Node node)
        {
            var mapNodeObject = Instantiate(nodePrefab, mapParent.transform);
            var mapNode = mapNodeObject.GetComponent<NodeView>();
            mapNode.Initialize(node, GetConfig(node.Type));
            mapNode.transform.localPosition = node.Position;
            return mapNode;
        }

        private void DrawLines()
        {
            foreach (var node in MapNodes)
            {
                foreach (var connection in node.Node.Outgoing)
                {
                    AddLineConnection(node, GetNode(connection));
                }
            }
        }

        public NodeView GetNode(Coordinate p)
        {
            return MapNodes.First(x => x.Node.Coordinate.Equals(p));
        }

        private void AddLineConnection(NodeView from, NodeView to)
        {
            var lineObject = Instantiate(linePrefab, mapParent.transform);
            var lineRenderer = lineObject.GetComponent<LineRenderer>();
            var fromPoint = from.transform.position -
                (to.transform.position - from.transform.position).normalized * offsetFromNodes;
            var toPoint = to.transform.position -
                (from.transform.position - to.transform.position).normalized * offsetFromNodes;

            lineObject.transform.position = fromPoint;
            lineRenderer.useWorldSpace = false;

            lineRenderer.positionCount = 2;
            for (var i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i,
                    Vector3.Lerp(Vector3.zero, toPoint - fromPoint, (float)i/(lineRenderer.positionCount-1)));
            }

            lineConnections.Add(new LineConnection(lineRenderer, from, to));
        }

        private void SetOrientation()
        {
            var scroll = mapParent.GetComponent<ScrollNonUI>();
            var span = map.DistanceBetweenFirstAndLastLayers();
            var bossNode = MapNodes.First(x => x.Node.Type == NodeType.Boss);

            firstParent.transform.position = new Vector3(
                mainCamera.transform.position.x, mainCamera.transform.position.y, 0f);
            var offset = orientationOffset;
            scroll.YConstraints.Max = 0;
            scroll.YConstraints.Min = -(span + 2f*offset);
            firstParent.transform.localPosition += new Vector3(0, offset, 0);
        }

        private NodeConfig GetConfig(NodeType type)
        {
            return config.Nodes.First(x => x.Type == type);
        }

        public void SetReachableNodes()
        {
            foreach (var node in MapNodes)
                node.SetState(NodeViewState.Locked);

            if (map.Path.Count == 0)
            {
                foreach (var node in MapNodes.Where(x => x.Node.Coordinate.Y == 0))
                    node.SetState(NodeViewState.Reachable);
            }
            else
            {
                foreach (var point in map.Path)
                {
                    var mapNode = GetNode(point);
                    if (mapNode != null)
                        mapNode.SetState(NodeViewState.Visited);
                }

                var currentPoint = map.Path[map.Path.Count-1];
                var currentNode = map.GetNode(currentPoint);

                foreach (var point in currentNode.Outgoing)
                {
                    var mapNode = GetNode(point);
                    if (mapNode != null)
                        mapNode.SetState(NodeViewState.Reachable);
                }
            }
        }

        public void SetLineColors()
        {
            foreach (var connection in lineConnections)
                connection.SetColor(lineLockedColor);

            if (map.Path.Count == 0)
                return;

            var currentCoordinate = map.Path[map.Path.Count-1];
            var currentNode = map.GetNode(currentCoordinate);

            foreach (var coordinate in currentNode.Outgoing)
            {
                var connection = lineConnections.First(x => x.From.Node == currentNode &&
                    x.To.Node.Coordinate.Equals(coordinate));
                connection.SetColor(lineVisitedColor);
            }

            if (map.Path.Count <= 1)
                return;

            for (var i = 0; i < map.Path.Count - 1; i++)
            {
                var current = map.Path[i];
                var next = map.Path[i+1];
                var connection = lineConnections.First(x => x.From.Node.Coordinate.Equals(current) &&
                    x.To.Node.Coordinate.Equals(next));
                connection.SetColor(lineVisitedColor);
            }
        }

        private void CreateBackground()
        {
            var go = new GameObject("Background");
            go.transform.SetParent(mapParent.transform);
            var bossNode = MapNodes.First(x => x.Node.Type == NodeType.Boss);
            var span = map.DistanceBetweenFirstAndLastLayers();
            go.transform.localPosition = new Vector3(bossNode.transform.localPosition.x, span/2f, 1f);
            go.transform.localRotation = Quaternion.identity;
            var sprite = go.AddComponent<SpriteRenderer>();
            sprite.drawMode = SpriteDrawMode.Sliced;
            sprite.sprite = background;
            sprite.size = new Vector2(xSize, span + yOffset * 2f);
            
            var newCamPos = mainCamera.transform.position;
            newCamPos.x = bossNode.transform.localPosition.x;
            mainCamera.transform.position = newCamPos;
        }
    }
}