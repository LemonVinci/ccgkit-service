// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    public class GameInfo : MonoBehaviour
    {
        public string ContextId = PlayContext.Kitchen;
        public string MapPrefKey = PlayContext.KitchenMapPref;
        public string SavePrefKey = PlayContext.KitchenSavePref;
        public string MapSceneName = "Map";

        public Coordinate PlayerCoordinate;
        
        public EncounterListEntry Encounter;
        public bool PlayerWonEncounter;

        /// <summary>When true, the Game scene ignores PlayerPrefs and uses the hero starting deck.</summary>
        public bool UseStartingDeck;

        public BattleTestSettings BattleTest = new BattleTestSettings();

        public bool HasBattleTestConfig => BattleTest != null && BattleTest.IsActive;

        public SaveData SaveData = new SaveData();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static GameInfo EnsureSession(string contextId, string mapPrefKey, string savePrefKey, string mapSceneName)
        {
            var existing = FindFirstObjectByType<GameInfo>();
            if (existing != null && existing.ContextId != contextId)
                Destroy(existing.gameObject);

            existing = FindFirstObjectByType<GameInfo>();
            if (existing == null)
            {
                var go = new GameObject("GameInfo");
                existing = go.AddComponent<GameInfo>();
            }

            existing.ContextId = contextId;
            existing.MapPrefKey = mapPrefKey;
            existing.SavePrefKey = savePrefKey;
            existing.MapSceneName = mapSceneName;
            return existing;
        }
    }
}