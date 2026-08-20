// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// Vertical log of ingredients played onto the dish (optional tiny art + name + value).
    /// </summary>
    public class DishPlateIngredientLog : MonoBehaviour
    {
        private const int MaxLines = 14;

        private RectTransform _contentRoot;
        private TMP_FontAsset _font;

        public void Configure(RectTransform contentRoot, TMP_FontAsset font)
        {
            _contentRoot = contentRoot;
            _font = font;
        }

        public void Clear()
        {
            if (_contentRoot == null)
                return;
            for (var i = _contentRoot.childCount - 1; i >= 0; i--)
                Destroy(_contentRoot.GetChild(i).gameObject);
        }

        public void Append(CardTemplate card, TMP_FontAsset fontOverride)
        {
            if (card == null || _contentRoot == null)
                return;

            while (_contentRoot.childCount >= MaxLines)
                Destroy(_contentRoot.GetChild(0).gameObject);

            var row = new GameObject("IngredientRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            row.transform.SetParent(_contentRoot, false);
            var rt = row.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0f, 28f);

            var rowLe = row.AddComponent<LayoutElement>();
            rowLe.minHeight = 28f;
            rowLe.preferredHeight = 28f;

            var h = row.GetComponent<HorizontalLayoutGroup>();
            h.childAlignment = TextAnchor.MiddleLeft;
            h.spacing = 6f;
            h.padding = new RectOffset(0, 0, 0, 0);
            h.childControlHeight = true;
            h.childControlWidth = false;
            h.childForceExpandHeight = false;
            h.childForceExpandWidth = false;

            if (card.Picture != null)
            {
                var iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image));
                iconGo.transform.SetParent(row.transform, false);
                var iconRt = iconGo.GetComponent<RectTransform>();
                iconRt.sizeDelta = new Vector2(28f, 28f);
                var iconLe = iconGo.AddComponent<LayoutElement>();
                iconLe.preferredWidth = 28f;
                iconLe.preferredHeight = 28f;
                var img = iconGo.GetComponent<Image>();
                img.sprite = card.Picture;
                img.preserveAspect = true;
                img.raycastTarget = false;
            }

            var textGo = new GameObject("Label", typeof(RectTransform));
            textGo.transform.SetParent(row.transform, false);
            var textLe = textGo.AddComponent<LayoutElement>();
            textLe.flexibleWidth = 1f;
            textLe.minWidth = 40f;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            var f = fontOverride != null ? fontOverride : _font;
            if (f != null)
                tmp.font = f;
            tmp.fontSize = 20f;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = RowColor(card.Suit);
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            tmp.text = $"{card.Name}  +{card.Value}";
        }

        private static Color RowColor(IngredientSuit suit)
        {
            switch (suit)
            {
                case IngredientSuit.Sweet: return new Color(1f, 0.55f, 0.6f);
                case IngredientSuit.Salty: return new Color(1f, 0.9f, 0.45f);
                case IngredientSuit.Sour: return new Color(0.55f, 1f, 0.65f);
                case IngredientSuit.Bitter: return new Color(0.55f, 0.75f, 1f);
                default: return Color.white;
            }
        }
    }
}
