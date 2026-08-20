// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    public class CardRewardView : MonoBehaviour
    {
        public Canvas Canvas;
        public Canvas PopupCanvas;
        public GameObject CardPrefab;
        public GameObject Content;

        private List<GameObject> widgets = new List<GameObject>(16);

        public void AddCards(CardLibrary cards)
        {
            var randomCards = new List<CardTemplate>();
            foreach (var entry in cards.Entries)
            {
                randomCards.Add(entry.Card);
            }
            randomCards.Shuffle();

            for (var i = 0; i < 3; i++)
            {
                var widget = Instantiate(CardPrefab);
                widget.transform.SetParent(Content.transform, false);
                widget.GetComponent<CardWidget>().SetInfo(randomCards[i]);
                widget.GetComponent<CardRewardWidget>().Canvas = Canvas;
                widget.GetComponent<CardRewardWidget>().PopupCanvas = PopupCanvas;
                widgets.Add(widget);
            }
        }

        private void OnEnable()
        {
            foreach (var widget in widgets)
            {
                Destroy(widget);
            }

            widgets.Clear();
        }

        private void OnDisable()
        {
        }
    }
}