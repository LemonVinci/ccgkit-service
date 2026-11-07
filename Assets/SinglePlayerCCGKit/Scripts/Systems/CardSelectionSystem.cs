// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The base class of the card selection systems. It contains the shared code of both
    /// selection systems (with and without a targeting arrow).
    /// </summary>
    public abstract class CardSelectionSystem : BaseSystem
    {
        public IntVariable PlayerMana;

        public TurnManagementSystem TurnManagementSystem;
        public DeckDrawingSystem DeckDrawingSystem;
        public HandPresentationSystem HandPresentationSystem;

        protected Camera MainCamera;
        protected LayerMask CardLayer;
        protected LayerMask TargetLayer;

        protected GameObject SelectedCard;

        protected ComboBattleSystem ComboBattleSystem;

        protected virtual void Start()
        {
            CardLayer = 1 << LayerMask.NameToLayer("Card");
            TargetLayer = 1 << LayerMask.NameToLayer("Enemy");
            MainCamera = Camera.main;
            ComboBattleSystem = FindFirstObjectByType<ComboBattleSystem>();
        }

        /// <summary>
        /// Whether the card has to be aimed with the targeting arrow. In the kitchen modes every
        /// ingredient is aimed at a dish station; in ComboGame only damage cards pick a monster.
        /// </summary>
        protected bool CardNeedsTarget(CardTemplate card)
        {
            if (ComboBattleSystem != null)
                return ComboBattleSystem.CardNeedsTarget(card);

            return CardUtils.CardHasTargetableEffect(card);
        }

        protected virtual void PlaySelectedCard()
        {
            var cardObject = SelectedCard.GetComponent<CardObject>();
            cardObject.SetInteractable(false);
            HandPresentationSystem.RearrangeHand(SelectedCard);
            HandPresentationSystem.RemoveCardFromHand(SelectedCard);

            // In the trick-taking prototype the card stays on the table until the trick is resolved.
            var trickTaking = FindFirstObjectByType<TrickTakingSystem>();
            if (trickTaking != null)
            {
                trickTaking.OnPlayerCardPlayed(SelectedCard);
                return;
            }

            HandPresentationSystem.MoveCardToDiscardPile(SelectedCard);

            DeckDrawingSystem.MoveCardToDiscardPile(cardObject.RuntimeCard);
        }

        public bool HasSelectedCard()
        {
            return SelectedCard != null;
        }
    }
}
