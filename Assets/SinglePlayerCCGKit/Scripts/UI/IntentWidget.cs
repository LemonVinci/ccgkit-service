// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// The intent widget that displays the associated enemy's current intent.
    /// </summary>
    public class IntentWidget : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private Image intentImage;
        [SerializeField]
        private TextMeshProUGUI amountText;
#pragma warning restore 649

        private bool isCharacterDead;

        private const float InitialDelay = 1.25f;
        private const float FadeInDuration = 0.8f;
        private const float FadeOutDuration = 0.5f;

        private void Start()
        {
            var transparentColor = intentImage.color;
            transparentColor.a = 0.0f;
            intentImage.color = transparentColor;
            amountText.color = transparentColor;
        }

        public void OnEnemyTurnBegan()
        {
            if (isCharacterDead)
            {
                return;
            }

            var seq = DOTween.Sequence();
            seq.AppendInterval(InitialDelay);
            seq.Append(intentImage.DOFade(0.0f, FadeOutDuration));

            seq = DOTween.Sequence();
            seq.AppendInterval(InitialDelay);
            seq.Append(amountText.DOFade(0.0f, FadeOutDuration));
        }

        public void OnIntentChanged(Sprite sprite, int value)
        {
            if (isCharacterDead)
            {
                return;
            }

            intentImage.sprite = sprite;
            intentImage.SetNativeSize();
            amountText.text = value.ToString();

            intentImage.DOFade(1.0f, FadeInDuration);
            amountText.DOFade(1.0f, FadeInDuration);
        }

        public void OnHpChanged(int hp)
        {
            if (hp <= 0)
            {
                isCharacterDead = true;
                intentImage.DOFade(0.0f, FadeOutDuration);
                amountText.DOFade(0.0f, FadeOutDuration);
            }
        }
    }
}
