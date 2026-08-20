// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CCGKit
{
    public class RepeatPattern : Pattern
    {
        public int Times;
        public Sprite Sprite;

        public override string GetName()
        {
            return $"Repeat x{Times.ToString()}";
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(2);

                GUILayout.BeginVertical("GroupBox", GUILayout.Width(200));
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(
                            new GUIContent("Times", "The number of times to repeat the effect/s."),
                            GUILayout.Width(EditorGUIUtility.labelWidth));
                        Times = EditorGUILayout.IntField(Times, GUILayout.Width(30));
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(
                            new GUIContent("Sprite", "The sprite associated to this effect."),
                            GUILayout.Width(EditorGUIUtility.labelWidth));
                        Sprite = (Sprite)EditorGUILayout.ObjectField(
                            Sprite, typeof(Sprite), false,
                            GUILayout.Width(50), GUILayout.Height(50));
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
#endif
    }
}
