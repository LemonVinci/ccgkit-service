// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Owns one or more dish stations per battle, applies ingredients to the targeted station,
    /// and tracks kitchen heat (increments each time the player ends a turn).
    /// </summary>
    public class DishSystem : MonoBehaviour
    {
        [Tooltip("When heat reaches or exceeds this value, the battle is lost (dish failed).")]
        [SerializeField]
        private int heatFailAt = 3;

        [Tooltip("Number of dish stations the player can cook on during a turn.")]
        [SerializeField]
        private int stationCount = 3;

        private readonly List<DishProgress> _stations = new List<DishProgress>(4);
        private bool _endedByHeat;
        private bool _endedBySuitOverflow;
        private int _heat;
        private int _focusedStationIndex;

        public int StationCount => _stations.Count;
        public bool HasStations => _stations.Count > 0 && _stations[0] != null;
        public int FocusedStationIndex => _focusedStationIndex;
        public DishProgress Progress => GetStationProgress(_focusedStationIndex);
        public bool HasActiveDish => Progress != null;
        public int CurrentHeat => _heat;
        public int HeatFailAt => heatFailAt;

        /// <summary>Invoked after init or each ingredient. Station index selects which plate changed.</summary>
        public event Action<int, DishProgress, CardTemplate> DishStateVisualChanged;

        public event Action<int, int> HeatChanged;

        public DishProgress GetStationProgress(int stationIndex)
        {
            if (stationIndex < 0 || stationIndex >= _stations.Count)
                return null;
            return _stations[stationIndex];
        }

        public void SetFocusedStation(int stationIndex)
        {
            if (stationIndex < 0 || stationIndex >= _stations.Count)
                return;

            _focusedStationIndex = stationIndex;
            RaiseDishVisual(stationIndex, _stations[stationIndex], null);
        }

        private bool _ignoreSuitOverflow;

        public void ApplyBattleTestSettings(BattleTestSettings settings)
        {
            if (settings == null)
                return;

            stationCount = Mathf.Clamp(settings.StationCount, 1, 3);
            _ignoreSuitOverflow = settings.IgnoreSuitOverflow;
            heatFailAt = settings.InfiniteHeat ? 999 : Mathf.Max(1, settings.HeatFailAt);
        }

        public void InitializeWithDish(Dish dish)
        {
            if (dish == null)
            {
                InitializeStations(null);
                return;
            }

            var count = Mathf.Max(1, stationCount);
            var dishes = new List<Dish>(count);
            for (var i = 0; i < count; i++)
                dishes.Add(dish);
            InitializeStations(dishes);
        }

        public void InitializeStations(IReadOnlyList<Dish> dishes)
        {
            _endedByHeat = false;
            _endedBySuitOverflow = false;
            _heat = 0;
            _focusedStationIndex = 0;
            _stations.Clear();

            if (dishes == null || dishes.Count == 0)
            {
                NotifyHeat();
                return;
            }

            for (var i = 0; i < dishes.Count; i++)
            {
                if (dishes[i] == null)
                    continue;
                _stations.Add(new DishProgress(dishes[i]));
            }

            for (var i = 0; i < _stations.Count; i++)
                RaiseDishVisual(i, _stations[i], null);

            NotifyHeat();
        }

        /// <summary>
        /// Applies an ingredient to a station. Returns false when the suit is not part of that dish’s recipe
        /// (no defeat, card should stay in hand).
        /// </summary>
        public bool TryRegisterIngredientPlayed(CardTemplate card, int stationIndex)
        {
            if (card == null || _endedByHeat || _endedBySuitOverflow)
                return false;

            var progress = GetStationProgress(stationIndex);
            if (progress == null)
                return false;

            if (!progress.CanAcceptIngredient(card.Suit))
                return false;

            _focusedStationIndex = stationIndex;
            progress.ApplyIngredient(card.Suit, card.Value);
            RaiseDishVisual(stationIndex, progress, card);

            if (!_ignoreSuitOverflow &&
                progress.TryGetFirstSuitOverTarget(out var overSuit, out var overTotal, out var overTarget))
            {
                _endedBySuitOverflow = true;
                var death = UnityEngine.Object.FindFirstObjectByType<CharacterDeathSystem>();
                if (death != null)
                    death.OnDishSuitOverTarget(overSuit, overTotal, overTarget);
            }

            return true;
        }

        public void RegisterIngredientPlayed(CardTemplate card, int stationIndex)
        {
            TryRegisterIngredientPlayed(card, stationIndex);
        }

        /// <summary>Legacy single-station entry point.</summary>
        public void RegisterIngredientPlayed(CardTemplate card)
        {
            RegisterIngredientPlayed(card, _focusedStationIndex);
        }

        public void OnPlayerTurnEnded()
        {
            if (!HasStations || _endedByHeat || _endedBySuitOverflow)
                return;

            _heat++;
            NotifyHeat();
            if (_heat < heatFailAt)
                return;

            _endedByHeat = true;
            var death = UnityEngine.Object.FindFirstObjectByType<CharacterDeathSystem>();
            if (death != null)
                death.OnDishOverheated();
        }

        private void RaiseDishVisual(int stationIndex, DishProgress progress, CardTemplate playedCard)
        {
            DishStateVisualChanged?.Invoke(stationIndex, progress, playedCard);
        }

        private void NotifyHeat()
        {
            HeatChanged?.Invoke(_heat, heatFailAt);
        }
    }
}
