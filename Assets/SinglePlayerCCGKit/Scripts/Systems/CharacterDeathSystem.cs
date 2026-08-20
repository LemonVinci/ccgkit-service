// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using UnityEngine;

namespace CCGKit
{
    public enum DishBattleDefeatKind
    {
        None,
        Heat,
        SuitOverTarget
    }

    /// <summary>
    /// Resolves the battle when the dish is completed (victory) or lost (heat or flavor over target).
    /// </summary>
    public class CharacterDeathSystem : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private float endBattlePopupDelay = 1.0f;
#pragma warning restore 649

        private DishBattleDefeatKind _pendingDefeatKind = DishBattleDefeatKind.None;
        private IngredientSuit _overflowSuit;
        private int _overflowTotal;
        private int _overflowTarget;

        public void OnDishComplete()
        {
            _pendingDefeatKind = DishBattleDefeatKind.None;
            EndGame(false);
        }

        public void OnDishOverheated()
        {
            _pendingDefeatKind = DishBattleDefeatKind.Heat;
            EndGame(true);
        }

        public void OnDishSuitOverTarget(IngredientSuit suit, int total, int target)
        {
            _pendingDefeatKind = DishBattleDefeatKind.SuitOverTarget;
            _overflowSuit = suit;
            _overflowTotal = total;
            _overflowTarget = target;
            EndGame(true);
        }

        public void EndGame(bool playerLost)
        {
            var turn = FindFirstObjectByType<TurnManagementSystem>();
            if (turn != null)
                turn.SetEndOfGame(true);

            StartCoroutine(ShowEndBattlePopup(playerLost));
        }

        private IEnumerator ShowEndBattlePopup(bool playerLost)
        {
            yield return new WaitForSeconds(endBattlePopupDelay);

            var popupOverlay = FindFirstObjectByType<PopupOverlay>();
            var endBattlePopup = FindFirstObjectByType<EndBattlePopup>();
            if (popupOverlay != null && endBattlePopup != null)
            {
                popupOverlay.Show();
                endBattlePopup.Show();

                var gameInfo = FindFirstObjectByType<GameInfo>();

                if (playerLost)
                {
                    if (_pendingDefeatKind == DishBattleDefeatKind.SuitOverTarget)
                        endBattlePopup.SetDefeatSuitOverTarget(_overflowSuit, _overflowTotal, _overflowTarget);
                    else
                        endBattlePopup.SetDefeatHeat();
                    if (gameInfo != null)
                        gameInfo.PlayerWonEncounter = false;
                }
                else
                {
                    endBattlePopup.SetVictoryText();
                    if (gameInfo != null)
                        gameInfo.PlayerWonEncounter = true;
                }
            }

            _pendingDefeatKind = DishBattleDefeatKind.None;
        }
    }
}
