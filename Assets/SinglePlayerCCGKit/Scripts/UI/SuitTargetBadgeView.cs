// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// One suit badge: icon + name + current/target (parent positions this in a diamond around the dish).
    /// </summary>
    public class SuitTargetBadgeView : MonoBehaviour
    {
        private Image _background;
        private TextMeshProUGUI _iconText;
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _valueText;

        public void Configure(Image background, TextMeshProUGUI iconText, TextMeshProUGUI nameText, TextMeshProUGUI valueText)
        {
            _background = background;
            _iconText = iconText;
            _nameText = nameText;
            _valueText = valueText;
        }

        public void SetActive(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void Bind(IngredientSuit suit, int current, int target)
        {
            _nameText.text = SuitLabel(suit);
            _iconText.text = SuitIcon(suit);
            _background.color = SuitBackground(suit);
            _valueText.text = $"{current} / {target}";
        }

        private static string SuitLabel(IngredientSuit suit)
        {
            switch (suit)
            {
                case IngredientSuit.Sweet: return "SWEET";
                case IngredientSuit.Salty: return "SALTY";
                case IngredientSuit.Sour: return "SOUR";
                case IngredientSuit.Bitter: return "BITTER";
                default: return suit.ToString().ToUpperInvariant();
            }
        }

        private static string SuitIcon(IngredientSuit suit)
        {
            switch (suit)
            {
                case IngredientSuit.Sweet: return "\u2665";
                case IngredientSuit.Salty: return "\u2660";
                case IngredientSuit.Sour: return "\u2666";
                case IngredientSuit.Bitter: return "\u2663";
                default: return string.Empty;
            }
        }

        private static Color SuitBackground(IngredientSuit suit)
        {
            switch (suit)
            {
                case IngredientSuit.Sweet: return new Color(0.85f, 0.35f, 0.4f, 0.95f);
                case IngredientSuit.Salty: return new Color(0.85f, 0.7f, 0.25f, 0.95f);
                case IngredientSuit.Sour: return new Color(0.25f, 0.65f, 0.35f, 0.95f);
                case IngredientSuit.Bitter: return new Color(0.2f, 0.3f, 0.55f, 0.95f);
                default: return new Color(0.3f, 0.3f, 0.3f, 0.9f);
            }
        }
    }
}
