// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace CCGKit
{
    /// <summary>
    /// The base class of all the targetable card effects that have an associated integer value
    /// (e.g., "Deal X damage", "Gain X HP").
    /// </summary>
    public abstract class IntegerEffect : TargetableEffect
    {
        public int Value;

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.fixedWidth = 100;
            var numberFieldStyle = new GUIStyle(EditorStyles.numberField);
            numberFieldStyle.fixedWidth = 40;

            Target = (EffectTargetType)EditorGUI.EnumPopup(rect, "Target", Target, popupStyle);
            rect.y += RowHeight;

            Value = EditorGUI.IntField(rect, "Value", Value, numberFieldStyle);
        }

        public override int GetNumRows()
        {
            return 3;
        }
#endif
    }
}
