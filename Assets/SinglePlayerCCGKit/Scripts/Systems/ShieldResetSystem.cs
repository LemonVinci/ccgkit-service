// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Resets the hero shield at the start of a player turn. No enemy combat; enemy shield hooks removed.
    /// </summary>
    public class ShieldResetSystem : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private PlayableCharacterConfiguration playerConfig;
#pragma warning restore 649

        public void OnPlayerTurnBegan()
        {
            if (playerConfig != null)
                playerConfig.Shield.SetValue(0);
        }
    }
}
