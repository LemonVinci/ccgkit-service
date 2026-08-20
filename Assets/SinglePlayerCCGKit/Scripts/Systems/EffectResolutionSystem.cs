// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Player ingredients no longer run combat/damage; progression is only via <see cref="DishSystem"/>.
    /// This type remains so scene and <see cref="EnemyBrainSystem"/> references stay valid, but
    /// resolution is intentionally empty.
    /// </summary>
    public class EffectResolutionSystem : BaseSystem
    {
        public void SetCurrentEnemy(CharacterObject enemy)
        {
        }

        public void ResolveCardEffects(RuntimeCard card, CharacterObject playerSelectedTarget)
        {
        }

        public void ResolveCardEffects(RuntimeCard card)
        {
        }

        public void ResolveEnemyEffects(CharacterObject enemy, List<Effect> effects)
        {
        }
    }
}
