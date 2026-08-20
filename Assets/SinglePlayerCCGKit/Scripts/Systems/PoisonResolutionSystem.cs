// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// Legacy hooks for poison/damage. Combat damage is disabled; use <see cref="DishSystem"/> for progression.
    /// </summary>
    public class PoisonResolutionSystem : BaseSystem
    {
        public void OnPlayerTurnBegan()
        {
        }

        public void OnPlayerTurnEnded()
        {
        }

        public void OnEnemyTurnBegan()
        {
        }

        public void OnEnemyTurnEnded()
        {
        }
    }
}
