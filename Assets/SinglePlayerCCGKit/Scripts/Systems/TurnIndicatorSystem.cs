// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using UnityEngine;
using TMPro;

namespace CCGKit
{
    public class TurnIndicatorSystem : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        public GameObject turnIndicator;
        [SerializeField]
        public TextMeshProUGUI text;
#pragma warning restore 649

        private const float InAnimDuration = 0.2f;
        private const float OutAnimDuration = 0.2f;
        private const float ShowcaseDuration = 1.0f;

        public void OnPlayerTurnBegan()
        {
            text.text = "Your turn";
            var seq = DOTween.Sequence();
            seq.Append(turnIndicator.GetComponent<RectTransform>().DOScale(0.5f, InAnimDuration));
            seq.AppendInterval(ShowcaseDuration);
            seq.Append(turnIndicator.GetComponent<RectTransform>().DOScale(0.0f, OutAnimDuration));
        }
    }
}

