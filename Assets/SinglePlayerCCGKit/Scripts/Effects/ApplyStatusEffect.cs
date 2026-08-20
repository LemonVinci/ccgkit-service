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
    /// The type corresponding to the "Apply X status" card effect.
    /// </summary>
    [Serializable]
    public class ApplyStatusEffect : IntegerEffect, IEntityEffect
    {
        public StatusTemplate Status;

        public override string GetName()
        {
            if (Status != null)
            {
                return $"Apply {Value.ToString()} {Status.Name}";
            }

            return "Apply status";
        }

        public override void Resolve(RuntimeCharacter instigator, RuntimeCharacter target)
        {
            var currentValue = target.Status.GetValue(Status.Name);
            target.Status.SetValue(Status, currentValue + Value);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.fixedWidth = 100;
            var numberFieldStyle = new GUIStyle(EditorStyles.numberField);
            numberFieldStyle.fixedWidth = 40;

            Target = (EffectTargetType)EditorGUI.EnumPopup(rect, "Target", Target, popupStyle);
            rect.y += RowHeight;

            Status = (StatusTemplate)EditorGUI.ObjectField(rect, "Status", Status, typeof(StatusTemplate), false);
            rect.y += RowHeight;

            Value = EditorGUI.IntField(rect, "Value", Value, numberFieldStyle);
        }

        public override int GetNumRows()
        {
            return 4;
        }
#endif
    }
}
