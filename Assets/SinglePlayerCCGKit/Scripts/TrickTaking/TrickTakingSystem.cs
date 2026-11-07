// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace CCGKit
{
    public enum TrickSeat
    {
        Player,
        Bot
    }

    /// <summary>
    /// Drives the trick-taking prototype: the player and a bot each play one card per round into a
    /// shared play zone. The follower "surpasses" when it answers with the same suit and a higher
    /// value; otherwise the leader keeps "leading" and the follower is left "pivoting". Whoever
    /// surpasses leads the next round, and when nobody does the previous leader starts again.
    /// </summary>
    public class TrickTakingSystem : MonoBehaviour
    {
        private enum RoundState
        {
            Idle,
            WaitingForPlayer,
            BotPlaying,
            ShowingResult
        }

#pragma warning disable 649
        [Header("Play zone")]
        [Tooltip("World position where the leading card is placed.")]
        [SerializeField]
        private Vector3 leadSlotPosition = new Vector3(-1.6f, 0.75f, 0f);

        [Tooltip("World position where the following card is placed.")]
        [SerializeField]
        private Vector3 followSlotPosition = new Vector3(1.6f, 0.75f, 0f);

        [SerializeField]
        private float cardMoveTime = 0.35f;

        [Header("Bot")]
        [SerializeField]
        private float botThinkDelay = 0.7f;

        [Header("Hands")]
        [SerializeField]
        private int botHandSize = 5;
#pragma warning restore 649

        private readonly List<CardTemplate> botDeck = new List<CardTemplate>(32);
        private readonly List<CardTemplate> botDiscard = new List<CardTemplate>(32);
        private readonly List<CardTemplate> botHand = new List<CardTemplate>(8);

        private DeckDrawingSystem deckDrawingSystem;
        private HandPresentationSystem handPresentationSystem;
        private ObjectPool cardPool;
        private TrickTakingResultPopup resultPopup;

        private RoundState state = RoundState.Idle;
        private TrickSeat startingSeat = TrickSeat.Player;
        private TrickSeat leadSeat = TrickSeat.Player;

        private CardTemplate leadCard;
        private CardTemplate followCard;

        private GameObject playerCardGo;
        private GameObject botCardGo;
        private RuntimeCard playerRuntimeCard;

        private readonly Vector3 tableCardScale = new Vector3(0.6f, 0.6f, 1.0f);

        /// <summary>True while the player is the one expected to drop a card on the play zone.</summary>
        public bool IsWaitingForPlayerCard => state == RoundState.WaitingForPlayer;

        public void Initialize(
            IReadOnlyList<CardTemplate> deckTemplates,
            DeckDrawingSystem deckDrawing,
            HandPresentationSystem handPresentation,
            ObjectPool pool)
        {
            deckDrawingSystem = deckDrawing;
            handPresentationSystem = handPresentation;
            cardPool = pool;

            botDeck.Clear();
            botDiscard.Clear();
            botHand.Clear();

            if (deckTemplates != null)
            {
                foreach (var template in deckTemplates)
                {
                    if (template != null)
                        botDeck.Add(template);
                }
            }

            botDeck.Shuffle();
            RefillBotHand();

            startingSeat = TrickSeat.Player;
            StartRound();
        }

        private void StartRound()
        {
            leadCard = null;
            followCard = null;
            playerCardGo = null;
            botCardGo = null;
            playerRuntimeCard = null;
            leadSeat = startingSeat;

            if (leadSeat == TrickSeat.Bot)
            {
                state = RoundState.BotPlaying;
                StartCoroutine(BotPlaysRoutine(true));
            }
            else
            {
                state = RoundState.WaitingForPlayer;
            }
        }

        /// <summary>Called by the card selection system once the player drops a card on the play zone.</summary>
        public void OnPlayerCardPlayed(GameObject cardGo)
        {
            if (cardGo == null || state != RoundState.WaitingForPlayer)
                return;

            var cardObject = cardGo.GetComponent<CardObject>();
            if (cardObject == null)
                return;

            playerCardGo = cardGo;
            playerRuntimeCard = cardObject.RuntimeCard;

            var playerIsLeading = leadSeat == TrickSeat.Player;
            if (playerIsLeading)
                leadCard = cardObject.Template;
            else
                followCard = cardObject.Template;

            PlaceCardOnTable(cardGo, playerIsLeading ? leadSlotPosition : followSlotPosition);

            if (playerIsLeading)
            {
                state = RoundState.BotPlaying;
                StartCoroutine(BotPlaysRoutine(false));
            }
            else
            {
                ResolveRound(TrickSeat.Player);
            }
        }

        private IEnumerator BotPlaysRoutine(bool botIsLeading)
        {
            yield return new WaitForSeconds(botThinkDelay);

            RefillBotHand();
            var chosen = botIsLeading ? ChooseBotLeadCard() : ChooseBotFollowCard(leadCard);

            if (chosen == null)
            {
                // Nothing left to play: keep the current starting seat and restart the round.
                state = RoundState.Idle;
                StartRound();
                yield break;
            }

            botHand.Remove(chosen);
            botDiscard.Add(chosen);

            if (botIsLeading)
                leadCard = chosen;
            else
                followCard = chosen;

            botCardGo = SpawnBotCard(chosen, botIsLeading ? leadSlotPosition : followSlotPosition);

            if (botIsLeading)
            {
                state = RoundState.WaitingForPlayer;
            }
            else
            {
                yield return new WaitForSeconds(cardMoveTime);
                ResolveRound(TrickSeat.Bot);
            }
        }

        private CardTemplate ChooseBotLeadCard()
        {
            CardTemplate best = null;
            foreach (var card in botHand)
            {
                if (best == null || card.Value > best.Value)
                    best = card;
            }
            return best;
        }

        /// <summary>Cheapest card that beats the lead on suit and value, or the lowest card when it cannot.</summary>
        private CardTemplate ChooseBotFollowCard(CardTemplate lead)
        {
            if (lead == null)
                return ChooseBotLeadCard();

            CardTemplate cheapestWinner = null;
            CardTemplate lowest = null;

            foreach (var card in botHand)
            {
                if (card.Suit == lead.Suit && card.Value > lead.Value)
                {
                    if (cheapestWinner == null || card.Value < cheapestWinner.Value)
                        cheapestWinner = card;
                }

                if (lowest == null || card.Value < lowest.Value)
                    lowest = card;
            }

            return cheapestWinner ?? lowest;
        }

        private void ResolveRound(TrickSeat follower)
        {
            state = RoundState.ShowingResult;

            var leader = leadSeat;
            var surpassed = leadCard != null && followCard != null &&
                            followCard.Suit == leadCard.Suit &&
                            followCard.Value > leadCard.Value;

            startingSeat = surpassed ? follower : leader;

            EnsureResultPopup();
            resultPopup.Show(leader, follower, leadCard, followCard, surpassed, EndRound);
        }

        private void EndRound()
        {
            DiscardTableCards();

            if (deckDrawingSystem != null && deckDrawingSystem.GetHandCount() == 0)
                deckDrawingSystem.DrawPlayerTurnHand();

            RefillBotHand();
            StartRound();
        }

        private void DiscardTableCards()
        {
            if (playerCardGo != null)
            {
                if (handPresentationSystem != null)
                    handPresentationSystem.MoveCardToDiscardPile(playerCardGo);
                if (deckDrawingSystem != null && playerRuntimeCard != null)
                    deckDrawingSystem.MoveCardToDiscardPile(playerRuntimeCard);
            }

            if (botCardGo != null)
            {
                var pooled = botCardGo.GetComponent<PooledObject>();
                botCardGo.transform.DOKill();
                if (pooled != null && pooled.Pool != null)
                    pooled.Pool.ReturnObject(botCardGo);
                else
                    Destroy(botCardGo);
            }

            playerCardGo = null;
            botCardGo = null;
            playerRuntimeCard = null;
        }

        private void PlaceCardOnTable(GameObject cardGo, Vector3 slotPosition)
        {
            var cardObject = cardGo.GetComponent<CardObject>();
            if (cardObject != null)
                cardObject.SetInteractable(false);

            cardGo.transform.DOKill();
            cardGo.transform.DOMove(slotPosition, cardMoveTime).SetEase(Ease.OutCubic);
            cardGo.transform.DORotate(Vector3.zero, cardMoveTime);
            cardGo.transform.DOScale(tableCardScale, cardMoveTime);

            var sorting = cardGo.GetComponent<SortingGroup>();
            if (sorting != null)
                sorting.sortingOrder = 60;
        }

        private GameObject SpawnBotCard(CardTemplate template, Vector3 slotPosition)
        {
            if (cardPool == null)
                return null;

            var go = cardPool.GetObject();
            var cardObject = go.GetComponent<CardObject>();
            if (cardObject != null)
            {
                cardObject.SetInfo(new RuntimeCard { Template = template });
                cardObject.SetInteractable(false);
                cardObject.SetGlowEnabled(false);
            }

            go.transform.DOKill();
            go.transform.position = slotPosition + new Vector3(0f, 2.5f, 0f);
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = tableCardScale;
            go.transform.DOMove(slotPosition, cardMoveTime).SetEase(Ease.OutCubic);

            var sorting = go.GetComponent<SortingGroup>();
            if (sorting != null)
                sorting.sortingOrder = 60;

            return go;
        }

        private void RefillBotHand()
        {
            while (botHand.Count < botHandSize)
            {
                if (botDeck.Count == 0)
                {
                    if (botDiscard.Count == 0)
                        return;

                    botDeck.AddRange(botDiscard);
                    botDiscard.Clear();
                    botDeck.Shuffle();
                }

                botHand.Add(botDeck[0]);
                botDeck.RemoveAt(0);
            }
        }

        private void EnsureResultPopup()
        {
            if (resultPopup != null)
                return;

            resultPopup = FindFirstObjectByType<TrickTakingResultPopup>();
            if (resultPopup == null)
            {
                var go = new GameObject("TrickTakingResultPopup");
                resultPopup = go.AddComponent<TrickTakingResultPopup>();
            }
        }
    }
}
