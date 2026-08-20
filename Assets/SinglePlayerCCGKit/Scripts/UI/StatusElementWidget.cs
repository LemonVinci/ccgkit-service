// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    public class StatusElementWidget : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private Image icon;

        [SerializeField]
        private TextMeshProUGUI text;
#pragma warning restore 649

        public string Type;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Initialize(StatusTemplate status, int value)
        {
            Type = status.Name;
            icon.sprite = status.Sprite;
            text.text = value.ToString();
        }

        public void Show()
        {
            canvasGroup.DOFade(1.0f, 1.0f);
        }

        public void UpdateModifier(int value)
        {
            text.text = value.ToString();
        }

        public void FadeAndDestroy()
        {
            var seq = DOTween.Sequence();
            seq.Append(icon.DOFade(0, 0.3f));
            seq.AppendCallback(() => Destroy(gameObject));

            text.DOFade(0, 0.3f);
        }
    }
}
