// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for detecting when a card containing a non-targeted effect
    /// is selected and played by the player.
    /// </summary>
    public class CardWithoutArrowSelectionSystem : CardSelectionSystem
    {
#pragma warning disable 649
        [SerializeField]
        private Canvas cardPileCanvas;

        [Tooltip("Enable in game modes where every card is played by dropping it on the play zone.")]
        [SerializeField]
        private bool playAnyCard;
#pragma warning restore 649

        public CardWithArrowSelectionSystem CardWithArrowSelectionSystem;

        private BoxCollider2D cardArea;
        private TrickTakingSystem trickTakingSystem;

        private Vector3 originalCardPos;
        private Quaternion originalCardRot;
        private int originalCardSortingOrder;

        private bool isCardAboutToBePlayed;
        
        private const float CardAnimationTime = 0.4f;
        private const float CardSelectionCanceledAnimationTime = 0.2f;
        private const float CardAboutToBePlayedOffsetY = 1.5f;
        private const Ease CardAnimationEase = Ease.OutBack;

        protected override void Start()
        {
            base.Start();
            var go = GameObject.Find("CardArea");
            if (go != null)
                cardArea = go.GetComponent<BoxCollider2D>();
            trickTakingSystem = FindFirstObjectByType<TrickTakingSystem>();
        }
        
        private void Update()
        {
            if (cardPileCanvas.isActiveAndEnabled)
                return;

            if (TurnManagementSystem.IsEndOfGame())
                return;

            // While the bot answers or the trick result is on screen the hand must stay locked.
            if (trickTakingSystem != null && !trickTakingSystem.IsWaitingForPlayerCard && SelectedCard == null)
                return;

            if (HandPresentationSystem.IsAnimating())
                return;

            if (CardWithArrowSelectionSystem.IsArrowActive())
                return;

            if (isCardAboutToBePlayed)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                DetectCardSelection();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                DetectCardUnselection();
            }

            if (SelectedCard != null)
                UpdateSelectedCard();
        }

        private void DetectCardSelection()
        {
            if (SelectedCard != null)
                return;
            
            // Checks if the player clicked over a card.
            var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hitInfo = Physics2D.Raycast(mousePos, Vector3.forward, Mathf.Infinity, CardLayer);
            if (hitInfo.collider != null)
            {
                var card = hitInfo.collider.GetComponent<CardObject>();
                var cardTemplate = card.Template;
                if (CardUtils.CardCanBePlayed(cardTemplate, PlayerMana) &&
                    (playAnyCard || !CardNeedsTarget(cardTemplate)))
                {
                    SelectedCard = hitInfo.collider.gameObject;
                    originalCardPos = SelectedCard.transform.position;
                    originalCardRot = SelectedCard.transform.rotation;
                    originalCardSortingOrder = SelectedCard.GetComponent<SortingGroup>().sortingOrder;
                }
            }
        }

        private void DetectCardUnselection()
        {
            if (SelectedCard != null)
            {
                var seq = DOTween.Sequence();
                seq.AppendCallback(() =>
                {
                    SelectedCard.GetComponent<CardObject>().SetState(CardObject.CardState.InHand);
                    SelectedCard.transform
                        .DOMove(originalCardPos, CardSelectionCanceledAnimationTime)
                        .SetEase(CardAnimationEase);
                    SelectedCard.transform.DORotate(originalCardRot.eulerAngles, CardSelectionCanceledAnimationTime);
                });
                seq.OnComplete(() =>
                {
                    SelectedCard.GetComponent<SortingGroup>().sortingOrder = originalCardSortingOrder;
                    SelectedCard = null;
                });
            }
        }

        private void UpdateSelectedCard()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var card = SelectedCard.GetComponent<CardObject>();
                if (card.State == CardObject.CardState.AboutToBePlayed)
                {
                    isCardAboutToBePlayed = true;
                    
                    var seq = DOTween.Sequence();
                    seq.Append(SelectedCard.transform
                        .DOMove(cardArea.bounds.center, CardAnimationTime)
                        .SetEase(CardAnimationEase));
                    seq.AppendInterval(CardAnimationTime + 0.1f);
                    seq.AppendCallback(() =>
                    {
                        PlaySelectedCard();
                        SelectedCard = null;
                        isCardAboutToBePlayed = false;
                    });
                    SelectedCard.transform.DORotate(Vector3.zero, CardAnimationTime);
                }
                else
                {
                    card.SetState(CardObject.CardState.InHand);
                    SelectedCard.GetComponent<CardObject>().Reset(() => SelectedCard = null);
                }
            }

            if (SelectedCard != null)
            {
                var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                SelectedCard.transform.position = mousePos;

                var card = SelectedCard.GetComponent<CardObject>();
                if (mousePos.y > originalCardPos.y + CardAboutToBePlayedOffsetY)
                    card.SetState(CardObject.CardState.AboutToBePlayed);
                else
                    card.SetState(CardObject.CardState.InHand);
            }
        }

        protected override void PlaySelectedCard()
        {
            var template = SelectedCard.GetComponent<CardObject>().Template;

            base.PlaySelectedCard();

            if (ComboBattleSystem != null)
                ComboBattleSystem.ResolvePlayedCard(template, null);
        }
    }
}
