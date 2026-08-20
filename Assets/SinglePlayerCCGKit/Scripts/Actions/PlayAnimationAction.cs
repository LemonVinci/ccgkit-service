// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CCGKit
{
    public class PlayAnimationAction : EffectAction
    {
        public Animator Animator;
        public Vector3 Offset;

        public override string GetName()
        {
            return "Play animation";
        }

        public override void Execute(GameObject go)
        {
            var pos = go.transform.position;
            var animator = Instantiate(Animator);
            animator.transform.position = pos + Offset;
            var autoKill = animator.gameObject.AddComponent<AutoKill>();
            autoKill.Duration = 2.0f;
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.fixedWidth = 100;
            var numberFieldStyle = new GUIStyle(EditorStyles.numberField);
            numberFieldStyle.fixedWidth = 40;

            Animator = (Animator)EditorGUI.ObjectField(rect, "Animator", Animator, typeof(Animator), false);
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
