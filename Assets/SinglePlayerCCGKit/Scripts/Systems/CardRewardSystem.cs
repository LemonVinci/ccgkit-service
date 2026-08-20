// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    public class CardRewardSystem : BaseSystem
    {
        public Canvas Canvas;
        public CardRewardView View;

        public CardLibrary RewardCards;

        public void OnPlayerRedeemedReward()
        {
            Canvas.gameObject.SetActive(true);
            View.AddCards(RewardCards);
        }
    }
}