// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// This component is associated to the end turn button. It does two things:
    ///     - It is automatically disabled when the button is pressed by the player.
    ///     - It is automatically enabled when the player's turn begins.
    /// </summary>
    public class EndTurnButton : MonoBehaviour
    {
        private Button button;

        private HandPresentationSystem handPresentationSystem;
        private CardWithArrowSelectionSystem cardWithArrowSelectionSystem;
        private CardWithoutArrowSelectionSystem cardWithoutArrowSelectionSystem;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            handPresentationSystem = FindFirstObjectByType<HandPresentationSystem>();
            cardWithArrowSelectionSystem = FindFirstObjectByType<CardWithArrowSelectionSystem>();
            cardWithoutArrowSelectionSystem = FindFirstObjectByType<CardWithoutArrowSelectionSystem>();
        }

        public void OnButtonPressed()
        {
            if (handPresentationSystem.IsAnimating())
            {
                return;
            }

            if (cardWithArrowSelectionSystem.HasSelectedCard() ||
                cardWithoutArrowSelectionSystem.HasSelectedCard())
            {
                return;
            }

            button.interactable = false;

            var system = GameObject.FindFirstObjectByType<TurnManagementSystem>();
            if (system == null || system.IsEndOfGame())
                return;
            system.EndPlayerTurn();
        }

        public void OnPlayerTurnBegan()
        {
            button.interactable = true;
        }
    }
}
