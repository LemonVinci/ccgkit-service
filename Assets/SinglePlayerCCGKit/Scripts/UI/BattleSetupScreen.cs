// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// Test battle launcher: configure plates, recipes, deck, heat, and start the Game scene.
    /// </summary>
    public class BattleSetupScreen : MonoBehaviour
    {
#pragma warning disable 649
        [Header("Encounter defaults")]
        [SerializeField]
        private string encounterName = "Test kitchen";
        [SerializeField]
        private Sprite encounterBackground;
        [SerializeField]
        private Dish defaultDish;

        [Header("Dish pool for random modes")]
        [SerializeField]
        private Dish[] dishPool;
#pragma warning restore 649

        private BattleTestSettings _settings;
        private TMP_FontAsset _font;
        private RectTransform _contentRoot;
        private TextMeshProUGUI _summaryLabel;
        private TextMeshProUGUI _baseDishValueLabel;

        private void Awake()
        {
            _settings = new BattleTestSettings
            {
                IsActive = true,
                StationCount = 3,
                RecipeMode = DishRecipeTestMode.AllSame,
                DeckMode = DeckTestMode.StartingDeck,
                HeatFailAt = 3,
                DeliverThresholdPercent = 85f,
                CardsDrawnPerTurn = 5,
                ShuffledSubsetSize = 12,
                DishPool = dishPool != null ? (Dish[])dishPool.Clone() : Array.Empty<Dish>()
            };
        }

        private void Start()
        {
            _font = TMP_Settings.defaultFontAsset;
            BuildUi();
            RefreshSummary();
        }

        private void BuildUi()
        {
            var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            CreateLabel(canvasGo.transform, "Title", "Configuración de combate (test)", 40, new Vector2(0, 490), new Vector2(1000, 56), FontStyles.Bold);
            AddFixedCombatButton(canvasGo.transform);

            var scrollGo = new GameObject("Scroll", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            scrollGo.transform.SetParent(canvasGo.transform, false);
            var scrollRt = scrollGo.GetComponent<RectTransform>();
            scrollRt.anchorMin = new Vector2(0.5f, 0.5f);
            scrollRt.anchorMax = new Vector2(0.5f, 0.5f);
            scrollRt.pivot = new Vector2(0.5f, 0.5f);
            scrollRt.sizeDelta = new Vector2(920, 640);
            scrollRt.anchoredPosition = new Vector2(0, 56);
            scrollGo.GetComponent<Image>().color = new Color(0.08f, 0.09f, 0.11f, 0.92f);

            var viewportGo = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewportGo.transform.SetParent(scrollGo.transform, false);
            var viewportRt = viewportGo.GetComponent<RectTransform>();
            viewportRt.anchorMin = Vector2.zero;
            viewportRt.anchorMax = Vector2.one;
            viewportRt.offsetMin = new Vector2(12, 12);
            viewportRt.offsetMax = new Vector2(-12, -12);
            viewportGo.GetComponent<Image>().color = new Color(0.12f, 0.13f, 0.16f, 1f);

            var contentGo = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            contentGo.transform.SetParent(viewportGo.transform, false);
            _contentRoot = contentGo.GetComponent<RectTransform>();
            _contentRoot.anchorMin = new Vector2(0, 1);
            _contentRoot.anchorMax = new Vector2(1, 1);
            _contentRoot.pivot = new Vector2(0.5f, 1);
            _contentRoot.anchoredPosition = Vector2.zero;
            _contentRoot.sizeDelta = new Vector2(0, 1200);

            var vlg = contentGo.GetComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(20, 20, 16, 16);
            vlg.spacing = 10;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var fitter = contentGo.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = scrollGo.GetComponent<ScrollRect>();
            scroll.viewport = viewportRt;
            scroll.content = _contentRoot;
            scroll.horizontal = false;
            scroll.vertical = true;

            AddSectionHeader("Platos");
            AddChoiceRow("Cantidad", new[] { "1 plato", "2 platos", "3 platos" }, _settings.StationCount - 1, i =>
            {
                _settings.StationCount = i + 1;
                RefreshSummary();
            });

            AddSectionHeader("Recetas");
            AddChoiceRow("Modo", new[] { "Todos iguales", "Todos random", "Random c/u" },
                (int)_settings.RecipeMode, i =>
                {
                    _settings.RecipeMode = (DishRecipeTestMode)i;
                    RefreshSummary();
                });

            _baseDishValueLabel = AddStepperRow("Plato base (si iguales)", _settings.BaseDishIndex, 0,
                Mathf.Max(0, GetDishPoolCount() - 1), v =>
                {
                    _settings.BaseDishIndex = v;
                    RefreshSummary();
                }, GetBaseDishName);

            AddSectionHeader("Mazo");
            AddChoiceRow("Cartas", new[]
                {
                    "Mazo inicial",
                    "Partida guardada",
                    "Solo dulce",
                    "Valor 4-5",
                    "Subconjunto"
                },
                (int)_settings.DeckMode, i =>
                {
                    _settings.DeckMode = (DeckTestMode)i;
                    RefreshSummary();
                });

            AddStepperRow("Tamaño subconjunto", _settings.ShuffledSubsetSize, 6, 20, v =>
            {
                _settings.ShuffledSubsetSize = v;
                RefreshSummary();
            });

            AddSectionHeader("Reglas");
            AddStepperRow("Calor máximo", _settings.HeatFailAt, 1, 8, v =>
            {
                _settings.HeatFailAt = v;
                RefreshSummary();
            });
            AddToggleRow("Calor infinito (no pierdes)", _settings.InfiniteHeat, v =>
            {
                _settings.InfiniteHeat = v;
                RefreshSummary();
            });
            AddToggleRow("Ignorar exceso de especia", _settings.IgnoreSuitOverflow, v =>
            {
                _settings.IgnoreSuitOverflow = v;
                RefreshSummary();
            });
            AddStepperRow("% para entregar", Mathf.RoundToInt(_settings.DeliverThresholdPercent), 50, 100, v =>
            {
                _settings.DeliverThresholdPercent = v;
                RefreshSummary();
            });
            AddStepperRow("Cartas por turno", _settings.CardsDrawnPerTurn, 1, 10, v =>
            {
                _settings.CardsDrawnPerTurn = v;
                RefreshSummary();
            });

            AddSectionHeader("Partida");
            AddToggleRow("Borrar save antes de jugar", _settings.ClearSaveBeforeBattle, v =>
            {
                _settings.ClearSaveBeforeBattle = v;
                RefreshSummary();
            });

            var summaryGo = CreateRow("Summary");
            _summaryLabel = summaryGo.AddComponent<TextMeshProUGUI>();
            ApplyFont(_summaryLabel, 20, FontStyles.Italic);
            _summaryLabel.alignment = TextAlignmentOptions.TopLeft;
            _summaryLabel.color = new Color(0.8f, 0.9f, 1f);

            AddActionButton("Ir al mapa", new Color(0.25f, 0.3f, 0.45f, 1f), () => Transition.LoadLevel("Map", 0.5f, Color.black));
        }

        private void AddFixedCombatButton(Transform canvas)
        {
            var barGo = new GameObject("StartCombatBar", typeof(RectTransform));
            barGo.transform.SetParent(canvas, false);
            var barRt = barGo.GetComponent<RectTransform>();
            barRt.anchorMin = new Vector2(0.5f, 0f);
            barRt.anchorMax = new Vector2(0.5f, 0f);
            barRt.pivot = new Vector2(0.5f, 0f);
            barRt.anchoredPosition = new Vector2(0, 28);
            barRt.sizeDelta = new Vector2(560, 80);

            var btnGo = new GameObject("StartCombatButton", typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(barGo.transform, false);
            var btnRt = btnGo.GetComponent<RectTransform>();
            btnRt.anchorMin = Vector2.zero;
            btnRt.anchorMax = Vector2.one;
            btnRt.offsetMin = Vector2.zero;
            btnRt.offsetMax = Vector2.zero;

            var img = btnGo.GetComponent<Image>();
            img.color = new Color(0.15f, 0.55f, 0.32f, 1f);
            var btn = btnGo.GetComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(StartCombat);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(btnGo.transform, false);
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;
            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            ApplyFont(labelTmp, 28, FontStyles.Bold);
            labelTmp.text = "Ir a escena de combate (Game)";
            labelTmp.alignment = TextAlignmentOptions.Center;
        }

        private int GetDishPoolCount()
        {
            var n = 0;
            if (dishPool == null)
                return defaultDish != null ? 1 : 0;
            foreach (var d in dishPool)
            {
                if (d != null)
                    n++;
            }
            return Mathf.Max(n, defaultDish != null ? 1 : 0);
        }

        private string GetBaseDishName(int index)
        {
            var list = GetPoolList();
            if (list.Length == 0)
                return "—";
            var dish = list[Mathf.Clamp(index, 0, list.Length - 1)];
            return dish != null ? dish.DisplayName : dish.name;
        }

        private Dish[] GetPoolList()
        {
            if (dishPool == null || dishPool.Length == 0)
                return defaultDish != null ? new[] { defaultDish } : Array.Empty<Dish>();
            return dishPool;
        }

        private void RefreshSummary()
        {
            if (_summaryLabel == null)
                return;

            var dishName = GetBaseDishName(_settings.BaseDishIndex);
            _summaryLabel.text =
                $"Platos: {_settings.StationCount} · Receta: {_settings.RecipeMode} · Mazo: {_settings.DeckMode}\n" +
                $"Calor: {(_settings.InfiniteHeat ? "∞" : _settings.HeatFailAt.ToString())} · Entregar: {_settings.DeliverThresholdPercent:0}% · Robo/turno: {_settings.CardsDrawnPerTurn}\n" +
                $"Plato base: {dishName} · Exceso especia: {(_settings.IgnoreSuitOverflow ? "perdonado" : "derrota")}";
        }

        public void StartCombat()
        {
            if (_settings.ClearSaveBeforeBattle)
            {
                PlayerPrefs.DeleteKey("save");
                PlayerPrefs.DeleteKey("map");
                PlayerPrefs.Save();
            }

            var gameInfo = FindFirstObjectByType<GameInfo>();
            if (gameInfo == null)
            {
                var go = new GameObject("GameInfo");
                gameInfo = go.AddComponent<GameInfo>();
            }

            gameInfo.ContextId = PlayContext.Kitchen;
            gameInfo.MapPrefKey = PlayContext.KitchenMapPref;
            gameInfo.SavePrefKey = PlayContext.KitchenSavePref;
            gameInfo.MapSceneName = "Map";

            var pool = GetPoolList();
            _settings.DishPool = pool;
            _settings.IsActive = true;

            gameInfo.Encounter = new EncounterListEntry
            {
                Name = encounterName,
                Background = encounterBackground,
                Dish = pool.Length > 0 ? pool[Mathf.Clamp(_settings.BaseDishIndex, 0, pool.Length - 1)] : defaultDish
            };
            gameInfo.PlayerWonEncounter = false;
            gameInfo.UseStartingDeck = _settings.DeckMode == DeckTestMode.StartingDeck;
            gameInfo.BattleTest = _settings.Clone();

            Transition.LoadLevel("Game", 0.5f, Color.black);
        }

        private void AddSectionHeader(string text)
        {
            var go = CreateRow("Header_" + text);
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 32;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            ApplyFont(tmp, 22, FontStyles.Bold);
            tmp.text = text;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = new Color(1f, 0.85f, 0.55f);
        }

        private void AddChoiceRow(string label, string[] options, int selectedIndex, Action<int> onChanged)
        {
            var row = CreateRow("Choice_" + label);
            var h = row.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 8;
            h.childAlignment = TextAnchor.MiddleLeft;
            h.childControlWidth = false;
            h.childForceExpandWidth = false;

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(row.transform, false);
            var labelLe = labelGo.AddComponent<LayoutElement>();
            labelLe.preferredWidth = 200;
            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            ApplyFont(labelTmp, 18, FontStyles.Normal);
            labelTmp.text = label;
            labelTmp.alignment = TextAlignmentOptions.MidlineLeft;

            var selected = selectedIndex;
            var chipImages = new Image[options.Length];

            for (var i = 0; i < options.Length; i++)
            {
                var idx = i;
                chipImages[i] = AddChipButton(row.transform, options[i], idx == selected, () =>
                {
                    selected = idx;
                    onChanged(idx);
                    for (var j = 0; j < chipImages.Length; j++)
                        SetChipHighlight(chipImages[j], j == selected);
                });
            }
        }

        private TextMeshProUGUI AddStepperRow(string label, int value, int min, int max, Action<int> onChanged, Func<int, string> formatValue = null)
        {
            var row = CreateRow("Stepper_" + label);
            var h = row.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 8;
            h.childAlignment = TextAnchor.MiddleLeft;

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(row.transform, false);
            var labelLe = labelGo.AddComponent<LayoutElement>();
            labelLe.preferredWidth = 260;
            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            ApplyFont(labelTmp, 18, FontStyles.Normal);
            labelTmp.text = label;
            labelTmp.alignment = TextAlignmentOptions.MidlineLeft;

            var current = value;
            var valueGo = new GameObject("Value", typeof(RectTransform));
            valueGo.transform.SetParent(row.transform, false);
            var valueLe = valueGo.AddComponent<LayoutElement>();
            valueLe.preferredWidth = 280;
            var valueTmp = valueGo.AddComponent<TextMeshProUGUI>();
            ApplyFont(valueTmp, 18, FontStyles.Bold);
            valueTmp.alignment = TextAlignmentOptions.MidlineLeft;

            void RefreshValue()
            {
                valueTmp.text = formatValue != null ? formatValue(current) : current.ToString();
            }

            RefreshValue();

            AddChipButton(row.transform, "−", false, () =>
            {
                current = Mathf.Max(min, current - 1);
                onChanged(current);
                RefreshValue();
            });
            AddChipButton(row.transform, "+", false, () =>
            {
                current = Mathf.Min(max, current + 1);
                onChanged(current);
                RefreshValue();
            });

            return valueTmp;
        }

        private void AddToggleRow(string label, bool value, Action<bool> onChanged)
        {
            var row = CreateRow("Toggle_" + label);
            var current = value;

            var go = new GameObject("Chip", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(row.transform, false);
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 36;
            le.flexibleWidth = 1;
            var img = go.GetComponent<Image>();
            SetChipHighlight(img, current);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var rt = labelGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(8, 4);
            rt.offsetMax = new Vector2(-8, -4);
            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            ApplyFont(labelTmp, 16, FontStyles.Normal);
            labelTmp.text = (current ? "[X] " : "[ ] ") + label;
            labelTmp.alignment = TextAlignmentOptions.Center;

            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                current = !current;
                onChanged(current);
                SetChipHighlight(img, current);
                labelTmp.text = (current ? "[X] " : "[ ] ") + label;
            });
        }

        private void AddActionButton(string text, Color color, Action onClick)
        {
            var row = CreateRow("Action_" + text);
            var le = row.AddComponent<LayoutElement>();
            le.preferredHeight = 56;

            var btnGo = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(row.transform, false);
            var btnLe = btnGo.AddComponent<LayoutElement>();
            btnLe.preferredHeight = 48;
            btnLe.flexibleWidth = 1;
            var img = btnGo.GetComponent<Image>();
            img.color = color;
            var btn = btnGo.GetComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => onClick());

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(btnGo.transform, false);
            var rt = labelGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            ApplyFont(tmp, 24, FontStyles.Bold);
            tmp.text = text;
            tmp.alignment = TextAlignmentOptions.Center;
        }

        private static readonly Color ChipNormalColor = new Color(0.22f, 0.24f, 0.28f, 1f);
        private static readonly Color ChipSelectedColor = new Color(0.2f, 0.55f, 0.35f, 1f);

        private Image AddChipButton(Transform parent, string text, bool selected, Action onClick)
        {
            var go = new GameObject("Chip", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 36;
            le.minWidth = 80;
            var img = go.GetComponent<Image>();
            SetChipHighlight(img, selected);
            var btn = go.GetComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => onClick());

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var rt = labelGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(8, 4);
            rt.offsetMax = new Vector2(-8, -4);
            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            ApplyFont(tmp, 16, FontStyles.Normal);
            tmp.text = text;
            tmp.alignment = TextAlignmentOptions.Center;
            return img;
        }

        private static void SetChipHighlight(Image img, bool selected)
        {
            if (img != null)
                img.color = selected ? ChipSelectedColor : ChipNormalColor;
        }

        private GameObject CreateRow(string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(_contentRoot, false);
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 40;
            le.preferredHeight = 44;
            le.flexibleWidth = 1;
            return go;
        }

        private static void CreateLabel(Transform parent, string name, string text, int fontSize, Vector2 position, Vector2 size, FontStyles style)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = position;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            if (TMP_Settings.defaultFontAsset != null)
                tmp.font = TMP_Settings.defaultFontAsset;
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private void ApplyFont(TextMeshProUGUI tmp, float size, FontStyles style)
        {
            if (_font != null)
                tmp.font = _font;
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.color = Color.white;
        }
    }
}
