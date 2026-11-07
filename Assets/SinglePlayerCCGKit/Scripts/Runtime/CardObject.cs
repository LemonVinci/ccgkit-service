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

        [Header("Combo presentation")]
        [SerializeField]
        private Sprite saltyFrame;
        [SerializeField]
        private Sprite bitterFrame;
        [SerializeField]
        private Sprite sweetFrame;
        [SerializeField]
        private Sprite sourFrame;
#pragma warning restore 649

        public RuntimeCard RuntimeCard;
        public CardTemplate Template;

        private SortingGroup sortingGroup;
        private SpriteRenderer frame;
        private Sprite defaultFrameSprite;
        private Color defaultTypeColor;
        private Color defaultInHandColor;
        private float defaultDescriptionSize;
        private bool defaultsCached;
        private bool comboPresentation;

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
            CachePresentationDefaults();
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
            descriptionText.text = Template.Description ?? string.Empty;
            picture.material = Template.Material;
            picture.sprite = Template.Picture;

            comboPresentation = FindFirstObjectByType<ComboBattleSystem>() != null;
            if (comboPresentation)
                ApplyComboPresentation();
            else
                RestoreDefaultPresentation();
        }

        private void CachePresentationDefaults()
        {
            if (defaultsCached)
                return;

            frame = GetComponent<SpriteRenderer>();
            if (frame != null)
                defaultFrameSprite = frame.sprite;
            if (typeText != null)
                defaultTypeColor = typeText.color;
            defaultInHandColor = inHandColor;
            if (descriptionText != null)
                defaultDescriptionSize = descriptionText.fontSize;
            defaultsCached = true;
        }

        private void ApplyComboPresentation()
        {
            CachePresentationDefaults();

            if (costText != null)
                costText.gameObject.SetActive(false);

            if (descriptionText != null)
                descriptionText.fontSize = Mathf.Max(defaultDescriptionSize, 2.4f);

            var accent = ComboAccent(Template.Suit);
            if (typeText != null)
            {
                typeText.color = accent;
                typeText.fontStyle = FontStyles.Bold;
            }

            if (frame != null)
            {
                var suitFrame = ComboFrame(Template.Suit);
                frame.sprite = suitFrame != null ? suitFrame : defaultFrameSprite;
                frame.color = Color.white;
            }

            inHandColor = accent;
            SetState(currState);
        }

        private void RestoreDefaultPresentation()
        {
            CachePresentationDefaults();

            if (costText != null)
                costText.gameObject.SetActive(true);

            if (descriptionText != null)
                descriptionText.fontSize = defaultDescriptionSize;

            if (typeText != null)
            {
                typeText.color = defaultTypeColor;
                typeText.fontStyle = FontStyles.Normal;
            }

            if (frame != null)
            {
                frame.sprite = defaultFrameSprite;
                frame.color = Color.white;
            }

            inHandColor = defaultInHandColor;
            SetState(currState);
        }

        private Sprite ComboFrame(IngredientSuit suit)
        {
            switch (suit)
            {
                case IngredientSuit.Salty:
                    return saltyFrame;
                case IngredientSuit.Bitter:
                    return bitterFrame;
                case IngredientSuit.Sweet:
                    return sweetFrame;
                case IngredientSuit.Sour:
                    return sourFrame;
                default:
                    return defaultFrameSprite;
            }
        }

        private static Color ComboAccent(IngredientSuit suit)
        {
            switch (suit)
            {
                case IngredientSuit.Salty:
                    return new Color(0.92f, 0.28f, 0.28f, 1f);
                case IngredientSuit.Bitter:
                    return new Color(0.62f, 0.66f, 0.72f, 1f);
                case IngredientSuit.Sweet:
                    return new Color(0.35f, 0.78f, 0.42f, 1f);
                case IngredientSuit.Sour:
                    return new Color(0.95f, 0.82f, 0.22f, 1f);
                default:
                    return Color.white;
            }
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
