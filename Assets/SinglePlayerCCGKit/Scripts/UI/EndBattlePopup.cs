// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// This component is attached to the EndBattlePopup prefab.
    /// </summary>
    public class EndBattlePopup : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private TextMeshProUGUI descriptionText;

        [SerializeField]
        private Button continueButton;
        [SerializeField]
        private Button rewardButton;

        [SerializeField]
        private GameEvent cardRewardEvent;
        [SerializeField]
        private Canvas popupCanvas;
#pragma warning restore 649

        private CanvasGroup canvasGroup;

        private const string VictoryText = "Victory";
        private const string DefeatText = "Defeat";
        private const string DefeatHeatDescription =
            "The kitchen ran too hot before you finished the dish. End turns sooner or complete the recipe faster.";
        private const float FadeInTime = 0.4f;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1.0f, FadeInTime);
        }

        public void SetVictoryText()
        {
            titleText.text = VictoryText;
            descriptionText.text = string.Empty;
        }

        public void SetDefeatText(string description = null)
        {
            titleText.text = DefeatText;
            descriptionText.text = description ?? string.Empty;
        }

        public void HideRewardButton()
        {
            if (rewardButton != null)
                Destroy(rewardButton.gameObject);
        }

        public void SetDefeatHeat()
        {
            ApplyDefeatChrome();
            titleText.text = DefeatText;
            descriptionText.text = DefeatHeatDescription;
        }

        public void SetDefeatSuitOverTarget(IngredientSuit suit, int total, int target)
        {
            ApplyDefeatChrome();
            titleText.text = DefeatText;
            var flavor = suit.ToString();
            descriptionText.text =
                $"You went over the recipe for {flavor}: you have {total} but the dish allows at most {target}. " +
                "Each flavor must stay at or below its target while you bring every required flavor up to its goal.";
        }

        private void ApplyDefeatChrome()
        {
            if (rewardButton != null)
                Destroy(rewardButton.gameObject);
            if (continueButton != null)
                continueButton.gameObject.SetActive(false);
        }

        public void OnContinueButtonPressed()
        {
            if (SceneManager.GetActiveScene().name == "ComboGame")
            {
                Transition.LoadLevel("ComboGame", 0.5f, Color.black);
                return;
            }

            var gameInfo = FindFirstObjectByType<GameInfo>();
            var mapScene = gameInfo != null && !string.IsNullOrEmpty(gameInfo.MapSceneName)
                ? gameInfo.MapSceneName
                : "Map";
            Transition.LoadLevel(mapScene, 0.5f, Color.black);
        }

        public void OnCardRewardButtonPressed()
        {
            Destroy(rewardButton.gameObject);
            popupCanvas.gameObject.SetActive(false);
            cardRewardEvent.Raise();
        }
    }
}
