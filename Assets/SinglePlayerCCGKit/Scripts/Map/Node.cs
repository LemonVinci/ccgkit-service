// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// A node is the most essential component of a map. Nodes represent a certain gameplay situation
    /// (enemy, boss, treasure, etc.) and are connected to each other in a randomly logical way.
    /// </summary>
    [Serializable]
    public class Node
    {
        public Coordinate Coordinate;
        public List<Coordinate> Incoming = new List<Coordinate>();
        public List<Coordinate> Outgoing = new List<Coordinate>();

        public NodeType Type;

        public Vector2 Position;

        public EncounterListEntry Encounter;

        public Node(NodeType type, Coordinate point)
        {
            Type = type;
            Coordinate = point;
        }

        public void AddIncoming(Coordinate p)
        {
            if (Incoming.Any(x => x.Equals(p)))
                return;

            Incoming.Add(p);
        }

        public void AddOutgoing(Coordinate p)
        {
            if (Outgoing.Any(x => x.Equals(p)))
                return;

            Outgoing.Add(p);
        }

        public void RemoveIncoming(Coordinate p)
        {
            Incoming.RemoveAll(x => x.Equals(p));
        }

        public void RemoveOutgoing(Coordinate p)
        {
            Outgoing.RemoveAll(x => x.Equals(p));
        }

        public bool HasNoConnections()
        {
            return Incoming.Count == 0 && Outgoing.Count == 0;
        }
    }
}