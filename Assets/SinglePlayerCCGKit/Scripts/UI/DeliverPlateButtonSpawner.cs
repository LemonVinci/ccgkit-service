// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// Spawns one "Deliver plate" button above each <see cref="DishTarget"/> station.
    /// </summary>
    public class DeliverPlateButtonSpawner : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private float requiredApproximationPercent = 85.0f;

        [Tooltip("World offset above each dish target for the deliver button.")]
        [SerializeField]
        private Vector3 buttonWorldOffset = new Vector3(0f, 2.1f, 0f);

        [SerializeField]
        private Vector2 buttonSize = new Vector2(240f, 72f);
#pragma warning restore 649

        private void Start()
        {
            StartCoroutine(SpawnWhenReady());
        }

        private IEnumerator SpawnWhenReady()
        {
            var endTurn = FindFirstObjectByType<EndTurnButton>();
            if (endTurn == null)
                yield break;

            var canvas = endTurn.GetComponentInParent<Canvas>();
            if (canvas == null)
                yield break;

            DishSystem dishSystem = null;
            while (true)
            {
                dishSystem = FindFirstObjectByType<DishSystem>();
                if (dishSystem != null && dishSystem.HasStations)
                {
                    var targets = FindObjectsOfType<DishTarget>();
                    if (targets.Length >= dishSystem.StationCount)
                        break;
                }

                yield return null;
            }

            var orderedTargets = FindObjectsOfType<DishTarget>()
                .OrderBy(t => t.StationIndex)
                .ToArray();

            var deliverThreshold = requiredApproximationPercent;
            var gameInfo = FindFirstObjectByType<GameInfo>();
            if (gameInfo != null && gameInfo.HasBattleTestConfig)
                deliverThreshold = gameInfo.BattleTest.DeliverThresholdPercent;

            for (var i = 0; i < orderedTargets.Length; i++)
                CreateButtonForStation(orderedTargets[i], endTurn, canvas, deliverThreshold);
        }

        private void CreateButtonForStation(DishTarget dishTarget, EndTurnButton template, Canvas canvas, float deliverThreshold)
        {
            var clone = Instantiate(template.gameObject, template.transform.parent);
            clone.name = "DeliverPlate_" + dishTarget.StationIndex;

            var rt = clone.GetComponent<RectTransform>();
            if (rt != null)
                rt.sizeDelta = buttonSize;

            var label = clone.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
            {
                label.text = "Deliver plate";
                label.fontSize = Mathf.Min(label.fontSize, 36f);
            }

            var button = clone.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                var handler = clone.GetComponent<DeliverPlateButton>();
                if (handler == null)
                    handler = clone.AddComponent<DeliverPlateButton>();
                handler.Initialize(dishTarget.StationIndex, deliverThreshold);
                button.onClick.AddListener(handler.OnButtonPressed);
            }

            var oldEndTurn = clone.GetComponent<EndTurnButton>();
            if (oldEndTurn != null)
                Destroy(oldEndTurn);

            var anchor = clone.GetComponent<WorldCanvasAnchor>();
            if (anchor == null)
                anchor = clone.AddComponent<WorldCanvasAnchor>();
            anchor.Configure(dishTarget.transform, buttonWorldOffset, canvas);
        }
    }

    /// <summary>
    /// Delivers a specific dish station when approximation meets the threshold.
    /// </summary>
    public class DeliverPlateButton : MonoBehaviour
    {
        private int stationIndex;
        private float requiredApproximation01;
        private HandPresentationSystem handPresentationSystem;
        private CardWithArrowSelectionSystem cardWithArrowSelectionSystem;
        private CardWithoutArrowSelectionSystem cardWithoutArrowSelectionSystem;

        public void Initialize(int stationIndex, float requiredApproximationPercent)
        {
            this.stationIndex = stationIndex;
            requiredApproximation01 = Mathf.Clamp01(requiredApproximationPercent / 100.0f);
        }

        private void Start()
        {
            handPresentationSystem = FindFirstObjectByType<HandPresentationSystem>();
            cardWithArrowSelectionSystem = FindFirstObjectByType<CardWithArrowSelectionSystem>();
            cardWithoutArrowSelectionSystem = FindFirstObjectByType<CardWithoutArrowSelectionSystem>();
        }

        public void OnButtonPressed()
        {
            if (handPresentationSystem != null && handPresentationSystem.IsAnimating())
                return;

            if ((cardWithArrowSelectionSystem != null && cardWithArrowSelectionSystem.HasSelectedCard()) ||
                (cardWithoutArrowSelectionSystem != null && cardWithoutArrowSelectionSystem.HasSelectedCard()))
                return;

            var turn = FindFirstObjectByType<TurnManagementSystem>();
            if (turn != null && turn.IsEndOfGame())
                return;

            var dishSystem = FindFirstObjectByType<DishSystem>();
            var progress = dishSystem != null
                ? dishSystem.GetStationProgress(stationIndex)
                : null;
            if (progress == null || progress.SourceDish == null)
                return;

            var approximation = DishScoring.ComputeAverage(progress.SourceDish, progress);
            if (approximation < requiredApproximation01)
            {
                Debug.Log(
                    $"Deliver plate {stationIndex + 1}: {Mathf.RoundToInt(approximation * 100f)}% is below required {Mathf.RoundToInt(requiredApproximation01 * 100f)}%.");
                return;
            }

            var death = FindFirstObjectByType<CharacterDeathSystem>();
            if (death != null)
                death.OnDishComplete();
        }
    }
}
