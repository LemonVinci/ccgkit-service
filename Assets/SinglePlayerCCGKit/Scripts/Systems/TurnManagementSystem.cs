// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// No-combat turn loop: only the player takes turns, repeatedly. "End turn" ends the
    /// current turn (fires <see cref="PlayerTurnEnded"/>) and immediately starts the next
    /// (fires <see cref="PlayerTurnBegan"/>). Outcomes are <see cref="DishSystem"/> completion (win)
    /// or heat (loss).
    /// </summary>
    public class TurnManagementSystem : MonoBehaviour
    {
        public GameEvent PlayerTurnBegan;
        public GameEvent PlayerTurnEnded;

        [SerializeField]
        private bool isEndOfGame;

        public void BeginGame()
        {
            BeginPlayerTurn();
        }

        public void BeginPlayerTurn()
        {
            if (isEndOfGame)
                return;
            PlayerTurnBegan.Raise();
        }

        public void EndPlayerTurn()
        {
            if (isEndOfGame)
                return;

            PlayerTurnEnded.Raise();
            if (isEndOfGame)
                return;

            BeginPlayerTurn();
        }

        public void SetEndOfGame(bool value)
        {
            isEndOfGame = value;
        }

        public bool IsEndOfGame()
        {
            return isEndOfGame;
        }
    }
}
