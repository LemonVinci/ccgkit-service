// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    public enum DishRecipeTestMode
    {
        AllSame,
        AllRandom,
        EachStationRandom
    }

    public enum DeckTestMode
    {
        StartingDeck,
        SavedDeck,
        SweetIngredientsOnly,
        HighValueOnly,
        ShuffledSubset
    }

    /// <summary>
    /// Options set from <see cref="BattleSetupScreen"/> and consumed by the Game scene for test battles.
    /// </summary>
    [Serializable]
    public class BattleTestSettings
    {
        public bool IsActive = true;
        public int StationCount = 3;
        public DishRecipeTestMode RecipeMode = DishRecipeTestMode.AllSame;
        public int BaseDishIndex;
        public DeckTestMode DeckMode = DeckTestMode.StartingDeck;
        public int HeatFailAt = 3;
        public bool InfiniteHeat;
        public bool IgnoreSuitOverflow;
        public float DeliverThresholdPercent = 85f;
        public int CardsDrawnPerTurn = 5;
        public int ShuffledSubsetSize = 12;
        public bool ClearSaveBeforeBattle;
        public Dish[] DishPool = Array.Empty<Dish>();

        public BattleTestSettings Clone()
        {
            var copy = (BattleTestSettings)MemberwiseClone();
            if (DishPool != null)
                copy.DishPool = (Dish[])DishPool.Clone();
            return copy;
        }
    }

    public static class BattleTestDishResolver
    {
        public static List<Dish> ResolveStations(BattleTestSettings settings, Dish encounterFallback)
        {
            var pool = BuildPool(settings, encounterFallback);
            var count = Mathf.Clamp(settings.StationCount, 1, 3);
            var result = new List<Dish>(count);

            if (pool.Count == 0)
                return result;

            switch (settings.RecipeMode)
            {
                case DishRecipeTestMode.AllSame:
                {
                    var dish = pool[Mathf.Clamp(settings.BaseDishIndex, 0, pool.Count - 1)];
                    for (var i = 0; i < count; i++)
                        result.Add(dish);
                    break;
                }
                case DishRecipeTestMode.AllRandom:
                {
                    for (var i = 0; i < count; i++)
                        result.Add(pool[UnityEngine.Random.Range(0, pool.Count)]);
                    break;
                }
                case DishRecipeTestMode.EachStationRandom:
                default:
                {
                    for (var i = 0; i < count; i++)
                        result.Add(pool[UnityEngine.Random.Range(0, pool.Count)]);
                    break;
                }
            }

            return result;
        }

        private static List<Dish> BuildPool(BattleTestSettings settings, Dish encounterFallback)
        {
            var pool = new List<Dish>();
            if (settings.DishPool != null)
            {
                foreach (var dish in settings.DishPool)
                {
                    if (dish != null && !pool.Contains(dish))
                        pool.Add(dish);
                }
            }

            if (pool.Count == 0 && encounterFallback != null)
                pool.Add(encounterFallback);

            return pool;
        }
    }
}
