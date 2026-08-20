// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using TMPro;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The widget used to display the player's mana.
    /// </summary>
    public class ManaWidget : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private TextMeshProUGUI textBorder;
#pragma warning restore 649

        private int maxValue;

        public void Initialize(IntVariable mana)
        {
            maxValue = mana.Value;
            SetValue(mana.Value);
        }

        private void SetValue(int value)
        {
            text.text = $"{value.ToString()}/{maxValue.ToString()}";
            textBorder.text = text.text;
        }

        public void OnManaChanged(int value)
        {
            SetValue(value);
        }
    }
}
