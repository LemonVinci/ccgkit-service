// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace CCGKit
{
    /// <summary>
    /// The type corresponding to the "Draw X cards" card effect.
    /// </summary>
    [Serializable]
    public class DrawCardsEffect : Effect
    {
        public int Amount;

        public override string GetName()
        {
            return $"Draw {Amount.ToString()} cards";
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var numberFieldStyle = new GUIStyle(EditorStyles.numberField);
            numberFieldStyle.fixedWidth = 40;

            Amount = EditorGUI.IntField(rect, "Amount", Amount, numberFieldStyle);
        }

        public override int GetNumRows()
        {
            return 2;
        }
#endif
    }
}
