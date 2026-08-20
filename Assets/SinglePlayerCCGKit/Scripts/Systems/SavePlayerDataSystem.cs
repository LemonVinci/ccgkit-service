// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// This system is responsible for keeping the current GameInfo's save data
    /// up-to-date.
    /// </summary>
    public class SavePlayerDataSystem : BaseSystem
    {
        public void OnPlayerHpChanged(int hp)
        {
            var gameInfo = FindFirstObjectByType<GameInfo>();
            gameInfo.SaveData.Hp = hp;
        }

        public void OnPlayerShieldChanged(int shield)
        {
            var gameInfo = FindFirstObjectByType<GameInfo>();
            gameInfo.SaveData.Shield = shield;
        }
    }
}