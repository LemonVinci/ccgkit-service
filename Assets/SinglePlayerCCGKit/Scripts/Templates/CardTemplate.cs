// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Scriptable template for a deck card. In this project it represents an <b>ingredient</b>:
    /// suit, numeric value, and an optional bonus <see cref="Effect"/> (e.g. draw, non-combat buffs).
    /// Deck, hand, and draw pipelines still use <see cref="RuntimeCard"/> with this template.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Templates/Ingredient",
        fileName = "Ingredient",
        order = 0)]
    public class CardTemplate : ScriptableObject
    {
        public int Id;
        public string Name;
        public IngredientSuit Suit;
        public int Value;
        [TextArea]
        public string Description;
        public Material Material;
        public Sprite Picture;

        /// <summary>Legacy sub-asset; not executed at play time. Use Remove in the custom editor to delete.</summary>
        public Effect BonusEffect;

        /// <summary>Unused at runtime (effects are not applied when playing a card). Kept for asset compatibility.</summary>
        public IEnumerable<Effect> EnumerateBonusEffects()
        {
            if (BonusEffect != null)
                yield return BonusEffect;
        }
    }
}
