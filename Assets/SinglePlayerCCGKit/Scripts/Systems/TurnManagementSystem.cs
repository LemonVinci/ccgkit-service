// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Drives the turn sequence. By default it runs a no-combat loop where only the player takes
    /// turns, repeatedly: "End turn" fires <see cref="PlayerTurnEnded"/> and immediately starts the
    /// next turn. When <see cref="enemyTurnsEnabled"/> is set (ComboGame), an enemy phase runs in
    /// between; whoever handles <see cref="EnemyPhaseBegan"/> is responsible for calling
    /// <see cref="EndEnemyTurn"/> once the monsters have acted.
    /// </summary>
    public class TurnManagementSystem : MonoBehaviour
    {
        public GameEvent PlayerTurnBegan;
        public GameEvent PlayerTurnEnded;

        [Tooltip("Enable in game modes that have an enemy phase between player turns.")]
        [SerializeField]
        private bool enemyTurnsEnabled;

        [SerializeField]
        private bool isEndOfGame;

        public event Action PlayerPhaseBegan;
        public event Action EnemyPhaseBegan;

        public void BeginGame()
        {
            BeginPlayerTurn();
        }

        public void BeginPlayerTurn()
        {
            if (isEndOfGame)
                return;
            PlayerTurnBegan.Raise();
            PlayerPhaseBegan?.Invoke();
        }

        public void EndPlayerTurn()
        {
            if (isEndOfGame)
                return;

            PlayerTurnEnded.Raise();
            if (isEndOfGame)
                return;

            if (enemyTurnsEnabled)
            {
                EnemyPhaseBegan?.Invoke();
                return;
            }

            BeginPlayerTurn();
        }

        public void EndEnemyTurn()
        {
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
