// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// Builds one dish + spice badge panel per <see cref="DishTarget"/> station, anchored above each plate.
    /// </summary>
    public class DishBattlePresenter : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private Canvas canvasOverride;

        [SerializeField]
        private Sprite dishSpriteOverride;

        [SerializeField]
        private TMP_FontAsset fontOverride;

        [Tooltip("World offset from each dish target for its UI panel.")]
        [SerializeField]
        private Vector3 panelWorldOffset = new Vector3(0f, 0.15f, 0f);
#pragma warning restore 649

        private sealed class StationUi
        {
            public SuitTargetBadgeView[] Badges;
            public DishPlateIngredientLog PlateLog;
        }

        private DishSystem _dishSystem;
        private TMP_FontAsset _font;
        private readonly Dictionary<int, StationUi> _stationUi = new Dictionary<int, StationUi>(4);

        private void Awake()
        {
            _dishSystem = GetComponent<DishSystem>();
            if (_dishSystem == null)
                _dishSystem = FindFirstObjectByType<DishSystem>();

            _font = fontOverride != null ? fontOverride : TMP_Settings.defaultFontAsset;

            if (_dishSystem != null)
                _dishSystem.DishStateVisualChanged += OnDishStateVisualChanged;

            StartCoroutine(BuildStationPanelsWhenReady());
        }

        private void OnDestroy()
        {
            if (_dishSystem != null)
                _dishSystem.DishStateVisualChanged -= OnDishStateVisualChanged;
        }

        private IEnumerator BuildStationPanelsWhenReady()
        {
            while (true)
            {
                if (_dishSystem != null && _dishSystem.HasStations)
                {
                    var targets = FindObjectsOfType<DishTarget>();
                    if (targets.Length >= _dishSystem.StationCount)
                        break;
                }

                yield return null;
            }

            var canvas = canvasOverride != null ? canvasOverride : FindBattleCanvas();
            if (canvas == null)
            {
                Debug.LogWarning("DishBattlePresenter: no canvas found.");
                yield break;
            }

            var orderedTargets = FindObjectsOfType<DishTarget>()
                .OrderBy(t => t.StationIndex)
                .ToArray();

            foreach (var dishTarget in orderedTargets)
                BuildStationPanel(dishTarget, canvas);

            for (var i = 0; i < _dishSystem.StationCount; i++)
                RefreshForStation(i, _dishSystem.GetStationProgress(i), null);
        }

        private void BuildStationPanel(DishTarget dishTarget, Canvas canvas)
        {
            var stationIndex = dishTarget.StationIndex;
            var rootGo = new GameObject("DishBattlePanel_" + stationIndex, typeof(RectTransform));
            var root = rootGo.GetComponent<RectTransform>();
            root.SetParent(canvas.transform, false);
            root.sizeDelta = new Vector2(300f, 380f);

            var anchor = rootGo.AddComponent<WorldCanvasAnchor>();
            anchor.Configure(dishTarget.transform, panelWorldOffset, canvas);

            var dishSprite = dishSpriteOverride != null ? dishSpriteOverride : Resources.Load<Sprite>("BattleDishArt");
            var dishGo = new GameObject("DishImage", typeof(RectTransform), typeof(Image));
            dishGo.transform.SetParent(root, false);
            var dishRt = dishGo.GetComponent<RectTransform>();
            dishRt.anchorMin = dishRt.anchorMax = dishRt.pivot = new Vector2(0.5f, 0.5f);
            dishRt.anchoredPosition = new Vector2(0f, 12f);
            dishRt.sizeDelta = new Vector2(200f, 140f);
            var dishImg = dishGo.GetComponent<Image>();
            dishImg.sprite = dishSprite;
            dishImg.preserveAspect = true;
            dishImg.raycastTarget = false;
            dishImg.color = dishSprite != null ? Color.white : new Color(0.25f, 0.22f, 0.2f, 0.85f);

            var badges = new SuitTargetBadgeView[4];
            badges[(int)IngredientSuit.Sweet] = CreateBadge(root, new Vector2(0f, 118f), new Vector2(108f, 72f), IngredientSuit.Sweet);
            badges[(int)IngredientSuit.Salty] = CreateBadge(root, new Vector2(-108f, 12f), new Vector2(108f, 72f), IngredientSuit.Salty);
            badges[(int)IngredientSuit.Sour] = CreateBadge(root, new Vector2(108f, 12f), new Vector2(108f, 72f), IngredientSuit.Sour);
            badges[(int)IngredientSuit.Bitter] = CreateBadge(root, new Vector2(0f, -98f), new Vector2(108f, 72f), IngredientSuit.Bitter);

            var plateGo = new GameObject("PlateLog", typeof(RectTransform), typeof(Image));
            plateGo.transform.SetParent(root, false);
            var plateRt = plateGo.GetComponent<RectTransform>();
            plateRt.anchorMin = new Vector2(0.5f, 0f);
            plateRt.anchorMax = new Vector2(0.5f, 0f);
            plateRt.pivot = new Vector2(0.5f, 0f);
            plateRt.anchoredPosition = new Vector2(0f, -168f);
            plateRt.sizeDelta = new Vector2(220f, 72f);
            var plateBg = plateGo.GetComponent<Image>();
            plateBg.color = new Color(0f, 0f, 0f, 0.35f);
            plateBg.raycastTarget = false;

            var contentGo = new GameObject("Content", typeof(RectTransform));
            contentGo.transform.SetParent(plateGo.transform, false);
            var contentRt = contentGo.GetComponent<RectTransform>();
            contentRt.anchorMin = Vector2.zero;
            contentRt.anchorMax = Vector2.one;
            contentRt.offsetMin = Vector2.zero;
            contentRt.offsetMax = Vector2.zero;
            var contentVlg = contentGo.AddComponent<VerticalLayoutGroup>();
            contentVlg.childAlignment = TextAnchor.UpperLeft;
            contentVlg.childControlHeight = true;
            contentVlg.childControlWidth = true;
            contentVlg.childForceExpandWidth = true;
            contentVlg.spacing = 0f;

            var plateLog = plateGo.AddComponent<DishPlateIngredientLog>();
            plateLog.Configure(contentRt, _font);

            _stationUi[stationIndex] = new StationUi
            {
                Badges = badges,
                PlateLog = plateLog
            };
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

        private SuitTargetBadgeView CreateBadge(RectTransform parent, Vector2 anchoredPosition, Vector2 size, IngredientSuit suit)
        {
            var go = new GameObject("Badge_" + suit, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = size;

            var bg = go.AddComponent<Image>();
            bg.raycastTarget = false;
            bg.color = Color.white;

            var vlg = go.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(4, 4, 4, 4);
            vlg.spacing = 0f;
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;

            var icon = CreateTmpLine(go.transform, "Icon", 26f, FontStyles.Normal);
            var name = CreateTmpLine(go.transform, "Suit", 12f, FontStyles.Bold);
            var val = CreateTmpLine(go.transform, "Value", 20f, FontStyles.Bold);

            var view = go.AddComponent<SuitTargetBadgeView>();
            view.Configure(bg, icon, name, val);
            return view;
        }

        private TextMeshProUGUI CreateTmpLine(Transform parent, string name, float height, FontStyles style)
        {
            var row = new GameObject(name, typeof(RectTransform));
            row.transform.SetParent(parent, false);
            var le = row.AddComponent<LayoutElement>();
            le.preferredHeight = height;
            le.minHeight = height;
            le.flexibleWidth = 1f;

            var tmp = row.AddComponent<TextMeshProUGUI>();
            if (_font != null)
                tmp.font = _font;
            tmp.fontSize = height - 2f;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            tmp.raycastTarget = false;
            tmp.color = Color.white;
            return tmp;
        }

        private void OnDishStateVisualChanged(int stationIndex, DishProgress progress, CardTemplate playedCard)
        {
            RefreshForStation(stationIndex, progress, playedCard);
        }

        private void RefreshForStation(int stationIndex, DishProgress progress, CardTemplate playedCard)
        {
            if (!_stationUi.TryGetValue(stationIndex, out var ui) || ui.Badges == null)
                return;

            if (progress == null)
            {
                foreach (var b in ui.Badges)
                {
                    if (b != null)
                        b.SetActive(false);
                }

                if (ui.PlateLog != null)
                    ui.PlateLog.Clear();
                return;
            }

            for (var i = 0; i < 4; i++)
            {
                var suit = (IngredientSuit)i;
                var target = progress.GetTarget(suit);
                var badge = ui.Badges[i];
                if (badge == null)
                    continue;
                if (target <= 0)
                {
                    badge.SetActive(false);
                    continue;
                }

                badge.SetActive(true);
                badge.Bind(suit, progress.GetTotal(suit), target);
            }

            if (playedCard != null && ui.PlateLog != null)
                ui.PlateLog.Append(playedCard, _font);
        }
    }
}
