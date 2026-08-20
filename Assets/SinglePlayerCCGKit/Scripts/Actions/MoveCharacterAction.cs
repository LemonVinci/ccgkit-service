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
    public class MoveCharacterAction : EffectAction
    {
        public float Duration;
        public Vector3 Offset;

        public override string GetName()
        {
            return "Move character";
        }

        public override void Execute(GameObject go)
        {
            var originalPos = go.transform.position;
            var seq = DOTween.Sequence();
            seq.Append(go.transform.DOMove(originalPos + Offset, Duration));
            seq.Append(go.transform.DOMove(originalPos, Duration * 2));
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var numberFieldStyle = new GUIStyle(EditorStyles.numberField);
            numberFieldStyle.fixedWidth = 40;

            Duration = EditorGUI.FloatField(rect, "Duration", Duration, numberFieldStyle);
            rect.y += RowHeight;

            Offset.x = EditorGUI.FloatField(rect, "X", Offset.x, numberFieldStyle);
            rect.y += RowHeight;
            Offset.y = EditorGUI.FloatField(rect, "Y", Offset.y, numberFieldStyle);
            rect.y += RowHeight;
            Offset.z = EditorGUI.FloatField(rect, "Z", Offset.z, numberFieldStyle);
        }

        public override int GetNumRows()
        {
            return 5;
        }
#endif
    }
}
