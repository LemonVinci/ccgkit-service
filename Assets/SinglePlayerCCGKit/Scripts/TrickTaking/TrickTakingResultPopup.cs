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
    /// Screen-space panel that reports the outcome of a trick: who leads, who surpassed and who pivots.
    /// The UI is built from code so the prototype scene needs no extra prefab wiring.
    /// </summary>
    public class TrickTakingResultPopup : MonoBehaviour
    {
        private Canvas canvas;
        private GameObject panel;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI bodyLabel;
        private Action onContinue;

        public void Show(
            TrickSeat leader,
            TrickSeat follower,
            CardTemplate leadCard,
            CardTemplate followCard,
            bool surpassed,
            Action continueCallback)
        {
            onContinue = continueCallback;
            EnsureUi();

            var leaderName = SeatName(leader);
            var followerName = SeatName(follower);

            if (surpassed)
            {
                titleLabel.text = followerName + " hizo Surpass";
                bodyLabel.text =
                    $"{followerName} respondió {Describe(followCard)} sobre {Describe(leadCard)} de {leaderName}.\n" +
                    $"{followerName} lidera la próxima ronda.";
            }
            else
            {
                titleLabel.text = leaderName + " está liderando";
                bodyLabel.text =
                    $"{leaderName} jugó {Describe(leadCard)} y {followerName} no pudo superarla con {Describe(followCard)}.\n" +
                    $"{followerName} queda pivotando. {leaderName} empieza la próxima ronda.";
            }

            panel.SetActive(true);
        }

        private static string SeatName(TrickSeat seat)
        {
            return seat == TrickSeat.Player ? "Tú" : "El bot";
        }

        private static string Describe(CardTemplate card)
        {
            if (card == null)
                return "ninguna carta";
            return $"{card.Suit} {card.Value}";
        }

        private void OnContinuePressed()
        {
            panel.SetActive(false);
            var callback = onContinue;
            onContinue = null;
            callback?.Invoke();
        }

        private void EnsureUi()
        {
            if (panel != null)
                return;

            var canvasGo = new GameObject("TrickTakingResultCanvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);

            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(canvasGo.transform, false);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(880f, 380f);
            panelRect.anchoredPosition = new Vector2(0f, 120f);
            panel.GetComponent<Image>().color = new Color(0.08f, 0.09f, 0.11f, 0.95f);

            titleLabel = CreateLabel(panelRect, "Title", 46, FontStyles.Bold,
                new Vector2(0f, 118f), new Vector2(820f, 70f));
            bodyLabel = CreateLabel(panelRect, "Body", 26, FontStyles.Normal,
                new Vector2(0f, 10f), new Vector2(820f, 150f));

            CreateContinueButton(panelRect);

            panel.SetActive(false);
        }

        private TextMeshProUGUI CreateLabel(RectTransform parent, string name, float fontSize, FontStyles style,
            Vector2 anchoredPosition, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var label = go.AddComponent<TextMeshProUGUI>();
            label.fontSize = fontSize;
            label.fontStyle = style;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            if (TMP_Settings.defaultFontAsset != null)
                label.font = TMP_Settings.defaultFontAsset;

            return label;
        }

        private void CreateContinueButton(RectTransform parent)
        {
            var btnGo = new GameObject("ContinueButton", typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(parent, false);

            var rect = btnGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(300f, 72f);
            rect.anchoredPosition = new Vector2(0f, -125f);

            var image = btnGo.GetComponent<Image>();
            image.color = new Color(0.2f, 0.45f, 0.62f, 1f);

            var button = btnGo.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(OnContinuePressed);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(btnGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.text = "Siguiente ronda";
            label.fontSize = 28;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            if (TMP_Settings.defaultFontAsset != null)
                label.font = TMP_Settings.defaultFontAsset;
        }
    }
}
