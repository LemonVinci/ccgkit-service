// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for resetting the hero's mana when their turn starts.
    /// </summary>
    public class ManaResetSystem : BaseSystem
    {
#pragma warning disable 649
        [SerializeField]
        private PlayableCharacterConfiguration playerConfig;
#pragma warning restore 649

        private int defaultMana;

        public void SetDefaultMana(int value)
        {
            defaultMana = value;
        }

        public void OnPlayerTurnBegan()
        {
            playerConfig.Mana.SetValue(defaultMana);
        }
    }
}
