// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Linq;
using UnityEngine;

using Random = System.Random;

namespace CCGKit
{
    /// <summary>
    /// This component keeps track of the player's progression through the given map.
    /// </summary>
    public class MapTracker : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private MapScreen mapScreen;
        [SerializeField]
        private MapView mapView;

        [SerializeField]
        private EncounterList easyEnemies;
        [SerializeField]
        private EncounterList hardEnemies;
        [SerializeField]
        private EncounterList eliteEnemies;
        [SerializeField]
        private EncounterList bosses;

        [SerializeField]
        private string gameSceneName = "Game";
#pragma warning restore 649

        private Map map;
        private Random rng;

        private int currEasyEnemyIdx;
        private int currHardEnemyIdx;
        private int currEliteEnemyIdx;

        public void TrackMap(Map map, Random rng)
        {
            this.map = map;
            this.rng = rng;
            FillMap();
        }

        private void FillMap()
        {
            easyEnemies.Encounters.Shuffle(rng);
            hardEnemies.Encounters.Shuffle(rng);
            bosses.Encounters.Shuffle(rng);

            foreach (var node in map.Nodes)
            {
                if (node.Type == NodeType.Enemy)
                {
                    if (node.Coordinate.Y == 0 || node.Coordinate.Y == 1)
                    {
                        node.Encounter = easyEnemies.Encounters[currEasyEnemyIdx];
                        currEasyEnemyIdx = (currEasyEnemyIdx + 1) % easyEnemies.Encounters.Count;
                    }
                    else
                    {
                        node.Encounter = hardEnemies.Encounters[currHardEnemyIdx];
                        currHardEnemyIdx = (currHardEnemyIdx + 1) % hardEnemies.Encounters.Count;
                    }
                }
                else if (node.Type == NodeType.Elite)
                {
                    node.Encounter = eliteEnemies.Encounters[currEliteEnemyIdx];
                    currEliteEnemyIdx = (currEliteEnemyIdx + 1) % eliteEnemies.Encounters.Count;
                }
                else if (node.Type == NodeType.Boss)
                {
                    node.Encounter = bosses.Encounters[0];
                }
            }
        }

        public void SelectNode(NodeView mapNode)
        {
            if (map.Path.Count == 0)
            {
                if (mapNode.Node.Coordinate.Y == 0)
                    TravelToNode(mapNode);
            }
            else
            {
                var coordinate = map.Path[map.Path.Count-1];
                var currentNode = map.GetNode(coordinate);
                if (currentNode != null && currentNode.Outgoing.Any(x => x.Equals(mapNode.Node.Coordinate)))
                    TravelToNode(mapNode);
            }
        }

        private void TravelToNode(NodeView mapNode)
        {
            var gameInfo = mapScreen != null ? mapScreen.EnsureSession() : GameInfo.EnsureSession(
                PlayContext.Kitchen, PlayContext.KitchenMapPref, PlayContext.KitchenSavePref, "Map");
            gameInfo.Encounter = mapNode.Node.Encounter;
            gameInfo.PlayerCoordinate = mapNode.Node.Coordinate;
            if (gameInfo.BattleTest != null)
                gameInfo.BattleTest.IsActive = false;

            Transition.LoadLevel(gameSceneName, 0.5f, Color.black);
        }
    }
}