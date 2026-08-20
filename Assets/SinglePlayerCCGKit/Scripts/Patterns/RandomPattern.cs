// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
#endif

namespace CCGKit
{
    public class RandomPattern : Pattern
    {
        public List<Probability> Probabilities = new List<Probability>(4);

#if UNITY_EDITOR
        private ReorderableList probabilitiesList;
        private Probability currentProbability;
#endif

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (probabilitiesList != null)
                return;

            probabilitiesList = new ReorderableList(Probabilities, typeof(Probability), true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Probabilities"); },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = Probabilities[index];
                    EditorGUI.LabelField(
                        new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), $"{element.Value}%");
                }
            };

            probabilitiesList.onSelectCallback = l =>
            {
                var selectedElement = Probabilities[probabilitiesList.index];
                currentProbability = selectedElement;
            };

            probabilitiesList.onAddDropdownCallback = (buttonRect, l) =>
            {
                var probability = new Probability();
                Probabilities.Add(probability);
            };

            probabilitiesList.onRemoveCallback = l =>
            {
                if (!EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this item?", "Yes", "No")
                )
                {
                    return;
                }
                var element = Probabilities[l.index];
                Probabilities.Remove(element);
                currentProbability = null;
            };
        }
#endif

        public override string GetName()
        {
            return "Randomize";
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(5);

                GUILayout.BeginVertical(GUILayout.Width(200));
                {
                    probabilitiesList?.DoLayoutList();
                }
                GUILayout.EndVertical();

                if (probabilitiesList != null)
                    DrawCurrentProbability();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawCurrentProbability()
        {
            if (currentProbability != null)
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10);

                    GUILayout.BeginVertical("GroupBox", GUILayout.Width(200));
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(
                                new GUIContent("Value", "The probability of this effect."),
                                GUILayout.Width(EditorGUIUtility.labelWidth));
                            currentProbability.Value = EditorGUILayout.IntField(
                                currentProbability.Value, GUILayout.Width(30));
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(
                                new GUIContent("Sprite", "The sprite associated to this effect."),
                                GUILayout.Width(EditorGUIUtility.labelWidth));
                            currentProbability.Sprite = (Sprite)EditorGUILayout.ObjectField(
                                currentProbability.Sprite, typeof(Sprite), false,
                                GUILayout.Width(50), GUILayout.Height(50));
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
        }
#endif
    }
}
