// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace CCGKit
{
    /// <summary>
    /// This component is linked to the actual GameObjects corresponding to the cards that
    /// are in the player's hand.
    /// </summary>
    public class CardObject : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private TextMeshPro costText;
        [SerializeField]
        private TextMeshPro nameText;
        [SerializeField]
        private TextMeshPro typeText;
        [SerializeField]
        private TextMeshPro descriptionText;

        [SerializeField]
        private SpriteRenderer picture;
        [SerializeField]
        private SpriteRenderer glow;

        [SerializeField] 
        private Color inHandColor;
        [SerializeField] 
        private Color aboutToBePlayedColor;
#pragma warning restore 649

        public RuntimeCard RuntimeCard;
        public CardTemplate Template;

        private SortingGroup sortingGroup;

        private Vector3 cachedPos;
        private Quaternion cachedRot;
        private int cachedSortingOrder;
        private int highlightedSortingOrder;

        private bool interactable;

        private HandPresentationSystem handPresentationSystem;
        private CardWithArrowSelectionSystem cardWithArrowSelectionSystem;
        private CardWithoutArrowSelectionSystem cardWithoutArrowSelectionSystem;

        private bool beingHighlighted;
        private bool beingUnhighlighted;

        public enum CardState
        {
            InHand,
            AboutToBePlayed
        }

        public CardState State => currState;
        private CardState currState;

        private void Awake()
        {
            sortingGroup = GetComponent<SortingGroup>();
            SetGlowEnabled(false);
        }

        private void Start()
        {
            handPresentationSystem = FindFirstObjectByType<HandPresentationSystem>();
            cardWithArrowSelectionSystem = FindFirstObjectByType<CardWithArrowSelectionSystem>();
            cardWithoutArrowSelectionSystem = FindFirstObjectByType<CardWithoutArrowSelectionSystem>();
        }

        private void OnEnable()
        {
            SetState(CardState.InHand);
        }

        public void SetInfo(RuntimeCard card)
        {
            RuntimeCard = card;
            Template = card.Template;
            costText.text = Template.Value.ToString();
            nameText.text = Template.Name;
            typeText.text = Template.Suit.ToString();
            descriptionText.text = string.Empty;
            picture.material = Template.Material;
            picture.sprite = Template.Picture;
        }

        public void SetGlowEnabled(bool glowEnabled)
        {
            glow.enabled = glowEnabled;
        }

        public void SetGlowEnabled(int playerMana)
        {
            glow.enabled = true;
        }

        public bool IsGlowEnabled()
        {
            return glow.enabled;
        }

        public void OnManaChanged(int mana)
        {
            SetGlowEnabled(mana);
        }

        public void SetState(CardState state)
        {
            currState = state;
            switch (currState)
            {
                case CardState.InHand:
                    glow.color = inHandColor;
                    break;
                
                case CardState.AboutToBePlayed:
                    glow.color = aboutToBePlayedColor;
                    break;
            }
        }

        public void SetInteractable(bool value)
        {
            interactable = value;
        }

        public void CacheTransform(Vector3 position, Quaternion rotation)
        {
            cachedPos = position;
            cachedRot = rotation;
            cachedSortingOrder = sortingGroup.sortingOrder;
            highlightedSortingOrder = cachedSortingOrder + 10;
        }

        private void OnMouseEnter()
        {
            if (interactable)
            {
                HighlightCard();
                handPresentationSystem.UnHighlightOtherCards(gameObject);
            }
        }

        private void OnMouseExit()
        {
            if (interactable)
            {
                UnHighlightCard();
            }
        }

        public void HighlightCard()
        {
            if (cardWithArrowSelectionSystem.HasSelectedCard() ||
                cardWithoutArrowSelectionSystem.HasSelectedCard())
            {
                return;
            }

            if (beingHighlighted)
            {
                return;
            }

            beingHighlighted = true;

            sortingGroup.sortingOrder = highlightedSortingOrder;
            transform.DOMove(cachedPos + new Vector3(0.0f, 1.0f, 0.0f), 0.05f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => beingHighlighted = false);
        } 

        public void UnHighlightCard()
        {
            if (cardWithArrowSelectionSystem.HasSelectedCard() ||
                cardWithoutArrowSelectionSystem.HasSelectedCard())
            {
                return;
            }

            if (beingUnhighlighted)
            {
                return;
            }

            beingUnhighlighted = true;

            sortingGroup.sortingOrder = cachedSortingOrder;
            var y = transform.position.y;
            var seq = DOTween.Sequence();
            transform.DOMove(cachedPos, 0.02f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => beingUnhighlighted = false);
        }

        public void Reset(Action onComplete)
        {
            transform.DOMove(cachedPos, 0.2f);
            transform.DORotateQuaternion(cachedRot, 0.2f);
            sortingGroup.sortingOrder = cachedSortingOrder;
            onComplete();
        }
    }
}
