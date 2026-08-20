// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Cumulative value contributed per <see cref="IngredientSuit"/>. Playing an ingredient adds its
    /// <see cref="CardTemplate.Value"/> to the matching suit total (no damage or combat).
    /// </summary>
    public sealed class DishProgress
    {
        private const int NumSuits = 4;
        private readonly Dish _dish;
        private readonly int[] _totals = new int[NumSuits];

        public DishProgress(Dish dish)
        {
            _dish = dish ?? throw new ArgumentNullException(nameof(dish));
        }

        public Dish SourceDish => _dish;

        public int GetTotal(IngredientSuit suit) => _totals[Mathf.Clamp((int)suit, 0, NumSuits - 1)];

        public int GetTarget(IngredientSuit suit) => _dish != null ? _dish.GetTarget(suit) : 0;

        /// <summary>True when this suit is part of the recipe (target &gt; 0).</summary>
        public bool IsSuitRequired(IngredientSuit suit) => GetTarget(suit) > 0;

        /// <summary>Ingredients may only be played for suits the dish requires.</summary>
        public bool CanAcceptIngredient(IngredientSuit suit) => IsSuitRequired(suit);

        /// <summary>0–1 toward this suit’s target. If target is 0, returns 0.</summary>
        public float GetSuitProgress01(IngredientSuit suit)
        {
            var t = _dish.GetTarget(suit);
            if (t <= 0)
                return 0f;
            return Mathf.Clamp01((float)GetTotal(suit) / t);
        }

        /// <summary>
        /// True when each suit with a non-zero target has total &gt;= target and none exceed their target
        /// (going over any required flavor ends the run separately).
        /// </summary>
        public bool IsComplete
        {
            get
            {
                if (_dish == null || HasAnyRequiredSuitOverTarget())
                    return false;
                for (var i = 0; i < NumSuits; i++)
                {
                    var s = (IngredientSuit)i;
                    var t = _dish.GetTarget(s);
                    if (t > 0 && GetTotal(s) < t)
                        return false;
                }

                return HasAnyRequiredTarget();
            }
        }

        /// <summary>Any required suit (target &gt; 0) whose running total is strictly greater than its target.</summary>
        public bool HasAnyRequiredSuitOverTarget()
        {
            if (_dish == null)
                return false;
            for (var i = 0; i < NumSuits; i++)
            {
                var s = (IngredientSuit)i;
                var t = _dish.GetTarget(s);
                if (t > 0 && GetTotal(s) > t)
                    return true;
            }

            return false;
        }

        /// <summary>First suit that is over target (for defeat messaging).</summary>
        public bool TryGetFirstSuitOverTarget(out IngredientSuit suit, out int total, out int target)
        {
            suit = default;
            total = 0;
            target = 0;
            if (_dish == null)
                return false;
            for (var i = 0; i < NumSuits; i++)
            {
                var s = (IngredientSuit)i;
                var t = _dish.GetTarget(s);
                if (t <= 0)
                    continue;
                var tot = GetTotal(s);
                if (tot > t)
                {
                    suit = s;
                    total = tot;
                    target = t;
                    return true;
                }
            }

            return false;
        }

        private bool HasAnyRequiredTarget()
        {
            for (var i = 0; i < NumSuits; i++)
            {
                if (_dish.GetTarget((IngredientSuit)i) > 0)
                    return true;
            }

            return false;
        }

        /// <summary>Adds <paramref name="value"/> to the total for <paramref name="suit"/>.</summary>
        public void ApplyIngredient(IngredientSuit suit, int value)
        {
            if (!CanAcceptIngredient(suit) || value <= 0)
                return;
            var i = Mathf.Clamp((int)suit, 0, NumSuits - 1);
            _totals[i] += value;
        }
    }
}
