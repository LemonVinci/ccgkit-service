// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    public class CardRewardWidget : MonoBehaviour
    {
        public Canvas Canvas;
        public Canvas PopupCanvas;

        private CardWidget cardWidget;

        private void Awake()
        {
            cardWidget = GetComponent<CardWidget>();
        }

        public void OnCardPressed()
        {
            var id =  cardWidget.Card.Id;
            var gameInfo = FindFirstObjectByType<GameInfo>();
            gameInfo.SaveData.Deck.Add(id);
            Canvas.gameObject.SetActive(false);
            PopupCanvas.gameObject.SetActive(true);
        }
    }
}