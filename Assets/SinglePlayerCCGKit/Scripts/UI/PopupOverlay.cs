// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// This component is attached to the in-game popup overlay image.
    /// </summary>
    public class PopupOverlay : MonoBehaviour
    {
        private CanvasGroup canvasGroup;

        private const float FadeInTime = 0.4f;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1.0f, FadeInTime);
        }
    }
}
