// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for drawing cards from the player's deck into the player's hand.
    /// Drawing cards from the deck can happen in the following situations:
    ///     - At the beginning of a game.
    ///     - At the beginning of the player's turn.
    ///     - As the result of a card effect ("Draw X cards").
    /// This system processes request-like entities with the component DrawCardsFromDeck attached to
    /// them. This component has an Amount field indicating the number of cards to draw. If there are
    /// not enough cards to draw from the deck, the discard pile is shuffled into the deck and cards
    /// are then drawn again from it.
    /// </summary>
    public class DeckDrawingSystem : MonoBehaviour
    {
        public HandPresentationSystem HandPresentationSystem;

        private List<RuntimeCard> deck;
        private List<RuntimeCard> discardPile;
        private List<RuntimeCard> hand;

        private DeckWidget deckWidget;
        private DiscardPileWidget discardPileWidget;

        // Change these values to the ones that make the most sense for your game.
        private const int InitialDeckCapacity = 30;
        private const int InitialDiscardPileCapacity = 30;
        private const int InitialHandCapacity = 30;

        public void Initialize(DeckWidget deck, DiscardPileWidget discardPile)
        {
            deckWidget = deck;
            discardPileWidget = discardPile;
            EnsureDeckLists();
        }

        private void Start()
        {
            EnsureDeckLists();
        }

        private void EnsureDeckLists()
        {
            if (deck != null)
                return;
            deck = new List<RuntimeCard>(InitialDeckCapacity);
            discardPile = new List<RuntimeCard>(InitialDiscardPileCapacity);
            hand = new List<RuntimeCard>(InitialHandCapacity);
        }

        public List<RuntimeCard> GetDeck()
        {
            return deck;
        }

        public List<RuntimeCard> GetDiscardPile()
        {
            return discardPile;
        }

        public int LoadDeck(List<CardTemplate> playerDeck)
        {
            EnsureDeckLists();
            deck.Clear();
            discardPile.Clear();
            hand.Clear();

            var deckSize = 0;

            foreach (var template in playerDeck)
            {
                // Skip over invalid entries.
                if (template == null)
                    continue;

                var card = new RuntimeCard 
                {
                    Template = template
                };
                deck.Add(card);

                ++deckSize;
            }

            deckWidget.SetAmount(deck.Count);
            discardPileWidget.SetAmount(0);

            return deckSize;
        }

        public void ShuffleDeck()
        {
            deck.Shuffle();
        }

        /// <summary>Draws the hand for a player turn; uses <see cref="BattleTestSettings.CardsDrawnPerTurn"/> when active.</summary>
        public void DrawPlayerTurnHand()
        {
            var amount = 5;
            var gameInfo = FindFirstObjectByType<GameInfo>();
            if (gameInfo != null && gameInfo.HasBattleTestConfig)
                amount = Mathf.Max(0, gameInfo.BattleTest.CardsDrawnPerTurn);
            DrawCardsFromDeck(amount);
        }

        public void DrawCardsFromDeck(int amount)
        {
            var deckSize = deck.Count;
            // If there are enough cards in the deck, just draw the cards from it.
            if (deckSize >= amount)
            {
                var prevDeckSize = deckSize;

                var drawnCards = new List<RuntimeCard>(amount);
                for (var i = 0; i < amount; i++)
                {
                    var card = deck[0];
                    deck.RemoveAt(0);
                    hand.Add(card);
                    drawnCards.Add(card);
                }

                HandPresentationSystem.CreateCardsInHand(drawnCards, prevDeckSize);
            }
            // If there are not enough cards in the deck, first shuffle the discard pile into
            // the deck and then draw from the deck normally.
            else
            {
                for (var i = 0; i < discardPile.Count; i++)
                    deck.Add(discardPile[i]);

                discardPile.Clear();

                HandPresentationSystem.UpdateDiscardPileSize(discardPile.Count);

                // Prevent trying to draw more cards than those available.
                if (amount > deck.Count + discardPile.Count)
                {
                    amount = deck.Count + discardPile.Count;
                }
                DrawCardsFromDeck(amount);
            }
        }

        public void MoveCardToDiscardPile(RuntimeCard card)
        {
            var idx = hand.IndexOf(card);
            hand.RemoveAt(idx);
            discardPile.Add(card);
        }

        public void MoveCardsToDiscardPile()
        {
            foreach (var card in hand)
                discardPile.Add(card);

            hand.Clear();
        }
    }
}
