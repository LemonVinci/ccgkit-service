// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for managing the AI of an enemy character, which mostly
    /// consists on following the patterns defined in the character's template.
    /// </summary>
    public class EnemyBrainSystem : BaseSystem
    {
#pragma warning disable 649
        [SerializeField]
        private EffectResolutionSystem effectResolutionSystem;

        [SerializeField]
        private List<IntentChangeEvent> intentChangeEvents;
#pragma warning restore 649

        private List<EnemyBrain> brains;

        private const float ThinkingTime = 1.5f;

        public override void Initialize(CharacterObject player, List<CharacterObject> enemies)
        {
            base.Initialize(player, enemies);
            brains = new List<EnemyBrain>(enemies.Count);
            for (var i = 0; i < enemies.Count; i++)
            {
                brains.Add(new EnemyBrain(enemies[i]));
            }
        }

        public void OnPlayerTurnBegan()
        {
            var enemyIdx = 0;
            foreach (var enemy in Enemies)
            {
                if (enemy.IsDead)
                {
                    enemyIdx++;
                    continue;
                }

                var template = enemy.Template as EnemyTemplate;
                var brain = brains[enemyIdx];

                if (brain.Pattern >= template.Patterns.Count)
                    brain.Pattern = 0;

                Sprite sprite = null;
                var pattern = template.Patterns[brain.Pattern];
                if (pattern is RepeatPattern repeatPattern)
                {
                    brain.Count += 1;
                    if (brain.Count == repeatPattern.Times)
                    {
                        brain.Count = 0;
                        brain.Pattern += 1;
                    }

                    brain.Effects = pattern.Effects;
                    sprite = repeatPattern.Sprite;
                }
                else if (pattern is RepeatForeverPattern repeatForeverPattern)
                {
                    brain.Effects = pattern.Effects;
                    sprite = repeatForeverPattern.Sprite;
                }
                else if (pattern is RandomPattern randomPattern)
                {
                    var effects = new List<int>(100);
                    var idx = 0;
                    foreach (var probability in randomPattern.Probabilities)
                    {
                        var amount = probability.Value;
                        for (var i = 0; i < amount; i++)
                        {
                            effects.Add(idx);
                        }

                        idx += 1;
                    }

                    var randomIdx = Random.Range(0, 100);
                    var selectedEffect = randomPattern.Effects[effects[randomIdx]];
                    brain.Effects = new List<Effect> { selectedEffect };

                    for (var i = 0; i < pattern.Effects.Count; i++)
                    {
                        var effect = pattern.Effects[i];
                        if (effect == selectedEffect)
                        {
                            sprite = randomPattern.Probabilities[i].Sprite;
                            break;
                        }
                    }

                    brain.Pattern += 1;
                }

                var currentEffect = brain.Effects[0];
                intentChangeEvents[enemyIdx++].Raise(sprite, (currentEffect as IntegerEffect).Value);
            }
        }

        public void OnEnemyTurnBegan()
        {
            StartCoroutine(ProcessEnemyBrains());
        }

        private IEnumerator ProcessEnemyBrains()
        {
            foreach (var brain in brains)
            {
                if (!brain.Enemy.IsDead)
                {
                    effectResolutionSystem.SetCurrentEnemy(brain.Enemy);
                    effectResolutionSystem.ResolveEnemyEffects(brain.Enemy, brain.Effects);
                    yield return new WaitForSeconds(ThinkingTime);
                }
            }
        }
    }
}
