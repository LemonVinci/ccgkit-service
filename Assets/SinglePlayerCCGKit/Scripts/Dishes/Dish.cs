// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// A dish: target amount per <see cref="IngredientSuit"/>. A value of 0 means that suit is not part
    /// of the recipe. Win when each non-zero target is reached exactly (totals must not go under or over).
    /// </summary>
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Dishes/Dish",
        fileName = "Dish",
        order = 0)]
    public class Dish : ScriptableObject
    {
        [Tooltip("Optional display name for UI.")]
        public string DisplayName;

        [Tooltip("Cumulative target for Sweet. 0 = not required.")]
        public int TargetSweet;
        [Tooltip("Cumulative target for Salty. 0 = not required.")]
        public int TargetSalty;
        [Tooltip("Cumulative target for Sour. 0 = not required.")]
        public int TargetSour;
        [Tooltip("Cumulative target for Bitter. 0 = not required.")]
        public int TargetBitter;

        public int GetTarget(IngredientSuit suit)
        {
            switch (suit)
            {
                case IngredientSuit.Sweet: return TargetSweet;
                case IngredientSuit.Salty: return TargetSalty;
                case IngredientSuit.Sour: return TargetSour;
                case IngredientSuit.Bitter: return TargetBitter;
                default: return 0;
            }
        }
    }
}
