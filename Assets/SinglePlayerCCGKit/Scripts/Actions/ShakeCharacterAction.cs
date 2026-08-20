// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CCGKit
{
    public class ShakeCharacterAction : EffectAction
    {
        public float Duration;
        public Vector3 Strength;

        public override string GetName()
        {
            return "Shake character";
        }

        public override void Execute(GameObject go)
        {
            go.transform.DOShakePosition(Duration, Strength);
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var numberFieldStyle = new GUIStyle(EditorStyles.numberField);
            numberFieldStyle.fixedWidth = 40;

            Duration = EditorGUI.FloatField(rect, "Duration", Duration, numberFieldStyle);
            rect.y += RowHeight;

            Strength.x = EditorGUI.FloatField(rect, "X", Strength.x, numberFieldStyle);
            rect.y += RowHeight;
            Strength.y = EditorGUI.FloatField(rect, "Y", Strength.y, numberFieldStyle);
            rect.y += RowHeight;
            Strength.z = EditorGUI.FloatField(rect, "Z", Strength.z, numberFieldStyle);
        }

        public override int GetNumRows()
        {
            return 5;
        }
#endif
    }
}
