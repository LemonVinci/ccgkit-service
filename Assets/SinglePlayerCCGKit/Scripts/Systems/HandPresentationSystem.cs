// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for managing the visual state and animations of the card
    /// game objects in the player's hand.
    /// </summary>
    public class HandPresentationSystem : MonoBehaviour
    {
        public IntVariable playerMana;

        private ObjectPool cardPool;
        private DeckWidget deckWidget;
        private DiscardPileWidget discardPileWidget;

        private readonly List<GameObject> handCards = new List<GameObject>(PositionsCapacity);

        private bool isAnimating;

        private List<Vector3> positions;
        private List<Quaternion> rotations;
        private List<int> sortingOrders;

        // Change these values to the ones that make the most sense for your game. These
        // must be in sync with the capacities defined in DeckDrawingSystem.
        private const int PositionsCapacity = 30;
        private const int RotationsCapacity = 30;
        private const int SortingOrdersCapacity = 30;

        private const float CenterRadius = 16.0f;
        private readonly Vector3 centerPoint = new Vector3(0.0f, -20.5f, 0.0f);
        private readonly Vector3 originalCardScale = new Vector3(0.6f, 0.6f, 1.0f);

        public static float CardToDiscardPileAnimationTime = 0.3f;

        public void Initialize(ObjectPool pool, DeckWidget deck, DiscardPileWidget discardPile)
        {
            cardPool = pool;
            deckWidget = deck;
            discardPileWidget = discardPile;
            EnsureLayoutLists();
        }

        private void Start()
        {
            EnsureLayoutLists();
        }

        private void EnsureLayoutLists()
        {
            if (positions != null)
                return;
            positions = new List<Vector3>(PositionsCapacity);
            rotations = new List<Quaternion>(RotationsCapacity);
            sortingOrders = new List<int>(SortingOrdersCapacity);
        }

        public void CreateCardsInHand(List<RuntimeCard> hand, int deckSize)
        {
            var drawnCards = new List<GameObject>(hand.Count);
            foreach (var card in hand)
            {
                var go = CreateCardGo(card);
                handCards.Add(go);
                drawnCards.Add(go);
            }

            deckWidget.SetAmount(deckSize);

            AnimateCardsFromDeckToHand(drawnCards);
        }

        public void UpdateDiscardPileSize(int size)
        {
            discardPileWidget.SetAmount(size);
        }

        public bool IsAnimating()
        {
            return isAnimating;
        }

        private GameObject CreateCardGo(RuntimeCard card)
        {
            var go = cardPool.GetObject();
            var obj = go.GetComponent<CardObject>();
            obj.SetInfo(card);
            obj.SetGlowEnabled(playerMana.Value);
            go.transform.position = deckWidget.transform.position;
            go.transform.localScale = Vector3.zero;

            return go;
        }

        private void AnimateCardsFromDeckToHand(List<GameObject> drawnCards)
        {
            isAnimating = true;

            ArrangeHandCards();

            foreach (var card in handCards)
            {
                card.GetComponent<CardObject>().SetInteractable(false);
            }

            var interval = 0.0f;
            for (var i = 0; i < handCards.Count; i++)
            {
                var i1 = i;
                const float time = 0.5f;
                var card = handCards[i];
                if (drawnCards.Contains(card))
                {
                    var cardObject = card.GetComponent<CardObject>();

                    var seq = DOTween.Sequence();
                    seq.AppendInterval(interval);
                    seq.AppendCallback(() =>
                    {
                        deckWidget.RemoveCard();
                        var move = card.transform.DOMove(positions[i1], time).OnComplete(() => {
                            cardObject.CacheTransform(positions[i1], rotations[i1]);
                            cardObject.SetInteractable(true);
                        });
                        card.transform.DORotateQuaternion(rotations[i1], time);
                        card.transform.DOScale(originalCardScale, time);
                        if (i1 == handCards.Count - 1)
                        {
                            move.OnComplete(() =>
                            {
                                isAnimating = false;
                                cardObject.CacheTransform(positions[i1], rotations[i1]);
                                cardObject.SetInteractable(true);
                            });
                        }
                    });

                    card.GetComponent<SortingGroup>().sortingOrder = sortingOrders[i];

                    interval += 0.2f;
                }
                else
                {
                    card.transform.DOMove(positions[i1], time).OnComplete(() => {
                        card.GetComponent<CardObject>().CacheTransform(positions[i1], rotations[i1]);
                        card.GetComponent<CardObject>().SetInteractable(true);
                    });
                    card.transform.DORotateQuaternion(rotations[i1], time);
                }
            }
        }

        private void ArrangeHandCards()
        {
            positions.Clear();
            rotations.Clear();
            sortingOrders.Clear();

            const float angle = 5.0f;
            var cardAngle = (handCards.Count - 1) * angle / 2;
            var z = 0.0f;
            for (var i = 0; i < handCards.Count; ++i)
            {
                // Rotate.
                var rotation = Quaternion.Euler(0, 0, cardAngle - i * angle);
                rotations.Add(rotation);

                // Move.
                z -= 0.1f;
                var position = CalculateCardPosition(cardAngle - i * angle);
                position.z = z;
                positions.Add(position);

                // Set sorting order.
                sortingOrders.Add(i);
            }
        }

        public void RearrangeHand(GameObject selectedCard)
        {
            handCards.Remove(selectedCard);

            ArrangeHandCards();

            for (var i = 0; i < handCards.Count; i++)
            {
                var card = handCards[i];
                const float time = 0.3f;
                card.transform.DOMove(positions[i], time);
                card.transform.DORotateQuaternion(rotations[i], time);
                card.GetComponent<SortingGroup>().sortingOrder = sortingOrders[i];
                card.GetComponent<CardObject>().SetGlowEnabled(playerMana.Value);
                card.GetComponent<CardObject>().CacheTransform(positions[i], rotations[i]);
            }
        }

        public void RemoveCardFromHand(GameObject go)
        {
            handCards.Remove(go);
        }

        public void MoveCardToDiscardPile(GameObject go)
        {
            var seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                go.transform.DOMove(discardPileWidget.transform.position, CardToDiscardPileAnimationTime);
                go.transform.DOScale(Vector3.zero, CardToDiscardPileAnimationTime).OnComplete(() =>
                {
                    go.GetComponent<PooledObject>().Pool.ReturnObject(go);
                });
            });
            seq.AppendCallback(() =>
            {
                discardPileWidget.AddCard();
                handCards.Remove(go);
            });
        }

        public void MoveHandToDiscardPile()
        {
            foreach (var card in handCards)
                MoveCardToDiscardPile(card);
            handCards.Clear();
        }

        private Vector3 CalculateCardPosition(float angle)
        {
            return new Vector3(
                centerPoint.x - CenterRadius * Mathf.Sin(Mathf.Deg2Rad * angle),
                centerPoint.y + CenterRadius * Mathf.Cos(Mathf.Deg2Rad * angle),
                0.0f);
        }

        public void UnHighlightOtherCards(GameObject x)
        {
            foreach (var card in handCards)
            {
                if (card != x)
                {
                    card.GetComponent<CardObject>().UnHighlightCard();
                }
            }
        }

        public void SetHandCardsInteractable(bool interactable)
        {
            foreach (var card in handCards)
            {
                card.GetComponent<CardObject>().SetInteractable(interactable);
            }
        }
    }
}
