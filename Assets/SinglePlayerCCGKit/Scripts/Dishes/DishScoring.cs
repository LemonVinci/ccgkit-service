// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CCGKit
{
    /// <summary>
    /// One required suit in a <see cref="Dish"/> with the current <see cref="DishScoring.ScoreSuit(int, int)"/> in 0..1.
    /// </summary>
    public readonly struct DishSuitScoreEntry
    {
        public DishSuitScoreEntry(IngredientSuit suit, int total, int target, float score)
        {
            Suit = suit;
            Total = total;
            Target = target;
            Score = score;
        }

        public IngredientSuit Suit { get; }
        public int Total { get; }
        public int Target { get; }
        public float Score { get; }
    }

    /// <summary>
    /// Reusable per-suit and aggregate scoring for a dish. Only suits with a positive target are included
    /// in the breakdown and the average.
    /// </summary>
    public static class DishScoring
    {
        private const int NumSuits = 4;

        /// <summary>
        /// For a single required suit: exact match 1.0, under = total/target, over = target/total.
        /// </summary>
        /// <param name="target">Required amount for this suit. Must be positive.</param>
        /// <returns>Score in (0, 1] for total &gt; 0; 0 if total and target are 0 is not allowed here — use <see cref="TryScoreSuit"/> for validation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="target"/> is not positive.</exception>
        public static float ScoreSuit(int total, int target)
        {
            if (target <= 0)
                throw new ArgumentOutOfRangeException(nameof(target), "Target must be positive.");
            if (total == target)
                return 1f;
            if (total < target)
                return (float)total / target;
            return (float)target / total;
        }

        /// <summary>
        /// Same rules as <see cref="ScoreSuit"/>. Returns false if <paramref name="target"/> is not positive
        /// (suit is not part of the recipe) — in that case <paramref name="score"/> is 0.
        /// </summary>
        public static bool TryScoreSuit(int total, int target, out float score)
        {
            if (target <= 0)
            {
                score = 0f;
                return false;
            }

            score = ScoreSuit(total, target);
            return true;
        }

        /// <summary>Entries for each suit in the recipe (target &gt; 0), in suit enum order (Sweet, Salty, Sour, Bitter).</summary>
        public static IReadOnlyList<DishSuitScoreEntry> BuildBreakdown(Dish dish, DishProgress progress)
        {
            if (dish == null)
                return Array.Empty<DishSuitScoreEntry>();
            if (progress == null)
                return Array.Empty<DishSuitScoreEntry>();

            var list = new List<DishSuitScoreEntry>(NumSuits);
            for (var i = 0; i < NumSuits; i++)
            {
                var suit = (IngredientSuit)i;
                var target = dish.GetTarget(suit);
                if (target <= 0)
                    continue;
                var total = progress.GetTotal(suit);
                var sc = ScoreSuit(total, target);
                list.Add(new DishSuitScoreEntry(suit, total, target, sc));
            }

            return list;
        }

        /// <summary>
        /// Mean of per-suit scores for all suits with a positive target. If there are no required suits, returns 0.
        /// </summary>
        public static float ComputeAverage(Dish dish, DishProgress progress)
        {
            var breakdown = BuildBreakdown(dish, progress);
            if (breakdown.Count == 0)
                return 0f;
            return (float)breakdown.Average(e => (double)e.Score);
        }

        /// <summary>
        /// Same scoring rules without a <see cref="DishProgress"/>; pass cumulative totals in suit order: Sweet, Salty, Sour, Bitter.
        /// </summary>
        public static float ComputeAverageFromTotals(Dish dish, int totalSweet, int totalSalty, int totalSour, int totalBitter)
        {
            if (dish == null)
                return 0f;

            int TotalForSuit(IngredientSuit s)
            {
                switch (s)
                {
                    case IngredientSuit.Sweet: return totalSweet;
                    case IngredientSuit.Salty: return totalSalty;
                    case IngredientSuit.Sour: return totalSour;
                    case IngredientSuit.Bitter: return totalBitter;
                    default: return 0;
                }
            }

            var sum = 0.0;
            var n = 0;
            for (var i = 0; i < NumSuits; i++)
            {
                var suit = (IngredientSuit)i;
                var target = dish.GetTarget(suit);
                if (target <= 0)
                    continue;
                sum += ScoreSuit(TotalForSuit(suit), target);
                n++;
            }

            if (n == 0)
                return 0f;
            return (float)(sum / n);
        }
    }
}
