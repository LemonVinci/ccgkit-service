// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// Simple TMP readout for kitchen heat: <c>current / max</c> (max is the fail threshold from <see cref="DishSystem"/>).
    /// Parents under the main battle canvas; wires to <see cref="DishSystem.HeatChanged"/>.
    /// </summary>
    public class DishHeatMeterPresenter : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private Canvas canvasOverride;

        [SerializeField]
        private TMP_FontAsset fontOverride;
#pragma warning restore 649

        private DishSystem _dishSystem;
        private TMP_FontAsset _font;
        private TextMeshProUGUI _label;

        private void Awake()
        {
            _dishSystem = GetComponent<DishSystem>();
            if (_dishSystem == null)
                _dishSystem = FindFirstObjectByType<DishSystem>();

            _font = fontOverride != null ? fontOverride : TMP_Settings.defaultFontAsset;
            BuildUi();
            if (_dishSystem != null)
            {
                _dishSystem.HeatChanged += OnHeatChanged;
                OnHeatChanged(_dishSystem.CurrentHeat, _dishSystem.HeatFailAt);
            }
        }

        private void OnDestroy()
        {
            if (_dishSystem != null)
                _dishSystem.HeatChanged -= OnHeatChanged;
        }

        private void BuildUi()
        {
            var canvas = canvasOverride != null ? canvasOverride : FindBattleCanvas();
            if (canvas == null)
                return;

            var go = new GameObject("HeatMeter", typeof(RectTransform));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(canvas.transform, false);
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-24f, -24f);
            rt.sizeDelta = new Vector2(220f, 40f);

            var bg = go.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.45f);
            bg.raycastTarget = false;

            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var trt = textGo.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = new Vector2(10f, 4f);
            trt.offsetMax = new Vector2(-10f, -4f);

            _label = textGo.AddComponent<TextMeshProUGUI>();
            if (_font != null)
                _label.font = _font;
            _label.fontSize = 26f;
            _label.fontStyle = FontStyles.Bold;
            _label.alignment = TextAlignmentOptions.MidlineRight;
            _label.color = new Color(1f, 0.82f, 0.55f);
            _label.raycastTarget = false;
            _label.text = "Heat 0 / 0";
        }

        private static Canvas FindBattleCanvas()
        {
            var canvases = FindObjectsOfType<Canvas>();
            foreach (var c in canvases)
            {
                if (c.renderMode != RenderMode.ScreenSpaceCamera)
                    continue;
                if (c.gameObject.name == "Canvas")
                    return c;
            }

            foreach (var c in canvases)
            {
                if (c.renderMode == RenderMode.ScreenSpaceCamera)
                    return c;
            }

            return canvases.Length > 0 ? canvases[0] : null;
        }

        private void OnHeatChanged(int current, int maxHeat)
        {
            if (_label == null)
                return;
            _label.text = $"Heat {current} / {maxHeat}";
        }
    }
}
