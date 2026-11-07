// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Battle rules for the ComboGame prototype, which brings back the original wizard-vs-monsters
    /// combat on top of the ingredient deck. Each suit maps to a combat effect:
    ///     - Salty: deal damage to the targeted monster.
    ///     - Bitter: gain shield.
    ///     - Sweet: increase shield gain for the rest of the turn.
    ///     - Sour: draw cards.
    /// Cards have no mana cost and there is no limit on how many can be played per turn.
    /// </summary>
    public class ComboBattleSystem : BaseSystem
    {
#pragma warning disable 649
        [Header("Suit effects")]
        [SerializeField]
        private int saltyDamage = 1;
        [SerializeField]
        private int bitterShield = 1;
        [SerializeField]
        private int sweetShieldBonus = 1;
        [SerializeField]
        private int sourCardsDrawn = 1;

        [Header("Systems")]
        [SerializeField]
        private TurnManagementSystem turnManagementSystem;
        [SerializeField]
        private DeckDrawingSystem deckDrawingSystem;

        [Header("Enemy turn")]
        [Tooltip("Intent widget events, one per enemy slot. Leave empty to skip intent display.")]
        [SerializeField]
        private List<IntentChangeEvent> intentChangeEvents = new List<IntentChangeEvent>();
        [SerializeField]
        private float enemyActionDelay = 0.9f;
        [SerializeField]
        private float endBattlePopupDelay = 1.0f;
#pragma warning restore 649

        private readonly List<EnemyPatternState> enemyStates = new List<EnemyPatternState>();

        private DealDamageEffect damageEffect;
        private GainShieldEffect shieldEffect;

        private int shieldBonusThisTurn;
        private bool battleResolved;

        /// <summary>Extra shield granted by Sweet cards played earlier in the current turn.</summary>
        public int ShieldBonusThisTurn => shieldBonusThisTurn;

        private class EnemyPatternState
        {
            public CharacterObject Enemy;
            public int PatternIndex;
            public int RepeatCount;
            public List<Effect> Effects = new List<Effect>();
        }

        public override void Initialize(CharacterObject player, List<CharacterObject> enemies)
        {
            base.Initialize(player, enemies);

            enemyStates.Clear();
            foreach (var enemy in enemies)
                enemyStates.Add(new EnemyPatternState { Enemy = enemy });

            if (turnManagementSystem != null)
            {
                turnManagementSystem.PlayerPhaseBegan += OnPlayerPhaseBegan;
                turnManagementSystem.EnemyPhaseBegan += OnEnemyPhaseBegan;
            }
        }

        private void OnDestroy()
        {
            if (turnManagementSystem == null)
                return;

            turnManagementSystem.PlayerPhaseBegan -= OnPlayerPhaseBegan;
            turnManagementSystem.EnemyPhaseBegan -= OnEnemyPhaseBegan;
        }

        /// <summary>Only damage cards ask the player to aim at a monster; the rest are played on the play area.</summary>
        public bool CardNeedsTarget(CardTemplate card)
        {
            return card != null && card.Suit == IngredientSuit.Salty && GetFirstAliveEnemy() != null;
        }

        public void ResolvePlayedCard(CardTemplate template, GameObject selectedTarget)
        {
            if (template == null || battleResolved || Player == null)
                return;

            EnsureEffects();

            switch (template.Suit)
            {
                case IngredientSuit.Salty:
                    DealDamageToEnemy(selectedTarget);
                    break;

                case IngredientSuit.Bitter:
                    shieldEffect.Value = bitterShield + shieldBonusThisTurn;
                    shieldEffect.Resolve(Player.Character, Player.Character);
                    break;

                case IngredientSuit.Sweet:
                    shieldBonusThisTurn += sweetShieldBonus;
                    break;

                case IngredientSuit.Sour:
                    StartCoroutine(DrawCardsAfterDiscardAnimation(sourCardsDrawn));
                    break;
            }

            CheckBattleOutcome();
        }

        private void DealDamageToEnemy(GameObject selectedTarget)
        {
            var target = selectedTarget != null ? selectedTarget.GetComponent<CharacterObject>() : null;
            if (target == null || target.IsDead)
                target = GetFirstAliveEnemy();
            if (target == null)
                return;

            damageEffect.Value = saltyDamage;
            damageEffect.Resolve(Player.Character, target.Character);
        }

        private IEnumerator DrawCardsAfterDiscardAnimation(int amount)
        {
            // The played card has to reach the discard pile before the hand is repopulated.
            yield return new WaitForSeconds(HandPresentationSystem.CardToDiscardPileAnimationTime + 0.1f);
            if (!battleResolved && deckDrawingSystem != null)
                deckDrawingSystem.DrawCardsFromDeck(amount);
        }

        public CharacterObject GetFirstAliveEnemy()
        {
            if (Enemies == null)
                return null;

            foreach (var enemy in Enemies)
            {
                if (enemy != null && !enemy.IsDead && enemy.Character != null && enemy.Character.Hp.Value > 0)
                    return enemy;
            }

            return null;
        }

        private void EnsureEffects()
        {
            if (damageEffect == null)
            {
                damageEffect = ScriptableObject.CreateInstance<DealDamageEffect>();
                damageEffect.Target = EffectTargetType.TargetEnemy;
            }

            if (shieldEffect == null)
            {
                shieldEffect = ScriptableObject.CreateInstance<GainShieldEffect>();
                shieldEffect.Target = EffectTargetType.Self;
            }
        }

        private void OnPlayerPhaseBegan()
        {
            if (battleResolved)
                return;

            shieldBonusThisTurn = 0;
            PrepareEnemyIntents();
        }

        private void OnEnemyPhaseBegan()
        {
            if (battleResolved)
                return;

            StartCoroutine(EnemyPhaseRoutine());
        }

        private IEnumerator EnemyPhaseRoutine()
        {
            foreach (var state in enemyStates)
            {
                if (battleResolved)
                    yield break;

                if (state.Enemy == null || state.Enemy.IsDead || state.Effects.Count == 0)
                    continue;

                state.Enemy.Character.Shield.SetValue(0);

                foreach (var effect in state.Effects)
                {
                    if (effect is TargetableEffect targetable)
                        ResolveEnemyEffect(state.Enemy, targetable);
                }

                CheckBattleOutcome();
                yield return new WaitForSeconds(enemyActionDelay);
            }

            if (!battleResolved && turnManagementSystem != null)
                turnManagementSystem.EndEnemyTurn();
        }

        private void ResolveEnemyEffect(CharacterObject enemy, TargetableEffect effect)
        {
            var target = effect.Target == EffectTargetType.Self ? enemy : Player;
            if (target == null)
                return;

            effect.Resolve(enemy.Character, target.Character);
        }

        /// <summary>
        /// Advances every enemy along its template patterns and publishes the resulting intent,
        /// mirroring the behaviour of <see cref="EnemyBrainSystem"/> in the original kit.
        /// </summary>
        private void PrepareEnemyIntents()
        {
            for (var i = 0; i < enemyStates.Count; i++)
            {
                var state = enemyStates[i];
                if (state.Enemy == null || state.Enemy.IsDead)
                    continue;

                var template = state.Enemy.Template as EnemyTemplate;
                if (template == null || template.Patterns.Count == 0)
                    continue;

                if (state.PatternIndex >= template.Patterns.Count)
                    state.PatternIndex = 0;

                var pattern = template.Patterns[state.PatternIndex];
                var sprite = SelectPatternEffects(state, pattern);

                RaiseIntent(i, state, sprite);
            }
        }

        private Sprite SelectPatternEffects(EnemyPatternState state, Pattern pattern)
        {
            switch (pattern)
            {
                case RepeatPattern repeat:
                    state.Effects = repeat.Effects;
                    state.RepeatCount += 1;
                    if (state.RepeatCount >= repeat.Times)
                    {
                        state.RepeatCount = 0;
                        state.PatternIndex += 1;
                    }
                    return repeat.Sprite;

                case RepeatForeverPattern repeatForever:
                    state.Effects = repeatForever.Effects;
                    return repeatForever.Sprite;

                case RandomPattern random:
                    return SelectRandomPatternEffect(state, random);

                default:
                    state.Effects = pattern.Effects;
                    state.PatternIndex += 1;
                    return null;
            }
        }

        private Sprite SelectRandomPatternEffect(EnemyPatternState state, RandomPattern random)
        {
            state.PatternIndex += 1;

            if (random.Effects.Count == 0)
            {
                state.Effects = new List<Effect>();
                return null;
            }

            var weighted = new List<int>(100);
            for (var i = 0; i < random.Probabilities.Count && i < random.Effects.Count; i++)
            {
                for (var j = 0; j < random.Probabilities[i].Value; j++)
                    weighted.Add(i);
            }

            var selectedIndex = weighted.Count > 0
                ? weighted[Random.Range(0, weighted.Count)]
                : Random.Range(0, random.Effects.Count);

            state.Effects = new List<Effect> { random.Effects[selectedIndex] };
            return selectedIndex < random.Probabilities.Count ? random.Probabilities[selectedIndex].Sprite : null;
        }

        private void RaiseIntent(int enemyIndex, EnemyPatternState state, Sprite sprite)
        {
            if (enemyIndex >= intentChangeEvents.Count || intentChangeEvents[enemyIndex] == null)
                return;
            if (state.Effects.Count == 0)
                return;

            var value = state.Effects[0] is IntegerEffect integerEffect ? integerEffect.Value : 0;
            intentChangeEvents[enemyIndex].Raise(sprite, value);
        }

        private void CheckBattleOutcome()
        {
            if (battleResolved || Player == null)
                return;

            if (Player.Character.Hp.Value <= 0)
            {
                EndBattle(true);
                return;
            }

            if (Enemies == null || Enemies.Count == 0)
                return;

            var everyEnemyDead = true;
            foreach (var enemy in Enemies)
            {
                if (enemy == null)
                    continue;

                if (enemy.Character.Hp.Value <= 0)
                {
                    if (!enemy.IsDead)
                    {
                        enemy.IsDead = true;
                        enemy.OnCharacterDied();
                    }
                }
                else
                {
                    everyEnemyDead = false;
                }
            }

            if (everyEnemyDead)
                EndBattle(false);
        }

        private void EndBattle(bool playerLost)
        {
            battleResolved = true;

            if (turnManagementSystem != null)
                turnManagementSystem.SetEndOfGame(true);

            StartCoroutine(ShowEndBattlePopup(playerLost));
        }

        private IEnumerator ShowEndBattlePopup(bool playerLost)
        {
            yield return new WaitForSeconds(endBattlePopupDelay);

            var popupOverlay = FindFirstObjectByType<PopupOverlay>();
            var endBattlePopup = FindFirstObjectByType<EndBattlePopup>();
            if (popupOverlay == null || endBattlePopup == null)
                yield break;

            popupOverlay.Show();
            endBattlePopup.Show();
            endBattlePopup.HideRewardButton();

            if (playerLost)
                endBattlePopup.SetDefeatText("The monsters brought you down. Try a different mix of ingredients.");
            else
                endBattlePopup.SetVictoryText();

            var gameInfo = FindFirstObjectByType<GameInfo>();
            if (gameInfo != null)
                gameInfo.PlayerWonEncounter = !playerLost;
        }
    }
}
