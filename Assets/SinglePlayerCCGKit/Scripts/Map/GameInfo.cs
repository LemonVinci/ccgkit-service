// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    public class GameInfo : MonoBehaviour
    {
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
    }
}