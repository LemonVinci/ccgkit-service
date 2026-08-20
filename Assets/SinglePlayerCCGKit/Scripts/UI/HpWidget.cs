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
    /// The widget used to display a character's HP and shield.
    /// </summary>
    public class HpWidget : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private Image hpBar;
        [SerializeField]
        private Image hpBarBackground;
        [SerializeField]
        private TextMeshProUGUI hpText;
        [SerializeField]
        private TextMeshProUGUI hpBorderText;
        [SerializeField]
        private GameObject shieldGroup;
        [SerializeField]
        private TextMeshProUGUI shieldText;
        [SerializeField]
        private TextMeshProUGUI shieldBorderText;
#pragma warning restore 649

        private int maxValue;

        public void Initialize(IntVariable hp, int max, IntVariable shield)
        {
            maxValue = max;
            SetHp(hp.Value, false);
            SetShield(shield.Value);
        }

        private void SetHp(int value, bool animateChange = true)
        {
            var newValue = value / (float)maxValue;
            if (animateChange)
            {
                hpBar.DOFillAmount(newValue, 0.2f)
                    .SetEase(Ease.InSine);

                var seq = DOTween.Sequence();
                seq.AppendInterval(0.5f);
                seq.Append(hpBarBackground.DOFillAmount(newValue, 0.2f));
                seq.SetEase(Ease.InSine);
            }
            else
            {
                hpBar.fillAmount = newValue;
                hpBarBackground.fillAmount = newValue;
            }

            hpText.text = $"{value.ToString()}/{maxValue.ToString()}";
            hpBorderText.text = $"{value.ToString()}/{maxValue.ToString()}";
        }

        private void SetShield(int value)
        {
            shieldText.text = $"{value.ToString()}";
            shieldBorderText.text = $"{value.ToString()}";
            SetShieldActive(value > 0);
        }

        private void SetShieldActive(bool shieldActive)
        {
            shieldGroup.SetActive(shieldActive);
        }

        public void OnHpChanged(int value)
        {
            SetHp(value);
            if (value <= 0)
            {
                gameObject.SetActive(false);
            }
        }

        public void OnShieldChanged(int value)
        {
            SetShield(value);
        }
    }
}
