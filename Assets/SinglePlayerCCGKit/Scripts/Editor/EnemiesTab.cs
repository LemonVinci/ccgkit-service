// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The "Enemies" tab in the editor.
    /// </summary>
    public class EnemiesTab : EditorTab
    {
        private EnemyTemplate currentEnemy;

        private ReorderableList patternsList;
        private Pattern currentPattern;

        private ReorderableList effectsList;
        private Effect currentEffect;

        public EnemiesTab(SinglePlayerCcgKitEditor editor) :
            base(editor)
        {
        }

        public override void Draw()
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;

            GUILayout.Space(15);

            var prevEnemy = currentEnemy;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                currentEnemy = (EnemyTemplate) EditorGUILayout.ObjectField(
                    "Asset", currentEnemy, typeof(EnemyTemplate), false, GUILayout.Width(340));
            }
            GUILayout.EndHorizontal();

            if (currentEnemy != null && currentEnemy != prevEnemy)
            {
                CreatePatternsList();
            }

            if (currentEnemy != null)
            {
                DrawCurrentEnemy();

                if (GUI.changed)
                    EditorUtility.SetDirty(currentEnemy);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private void DrawCurrentEnemy()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(10);

                GUILayout.BeginVertical("GroupBox", GUILayout.Width(100));
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Name", "The name of this enemy."),
                                GUILayout.Width(EditorGUIUtility.labelWidth));
                            currentEnemy.Name = EditorGUILayout.TextField(currentEnemy.Name, GUILayout.Width(150));
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Hp", "The initial HP of this enemy."),
                                GUILayout.Width(EditorGUIUtility.labelWidth));
                            currentEnemy.HpLow = EditorGUILayout.IntField(currentEnemy.HpLow, GUILayout.Width(30));
                            currentEnemy.HpHigh =
                                EditorGUILayout.IntField(currentEnemy.HpHigh, GUILayout.Width(30));
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Prefab", "The prefab of this enemy."),
                                GUILayout.Width(EditorGUIUtility.labelWidth));
                            currentEnemy.Prefab = (GameObject) EditorGUILayout.ObjectField(
                                currentEnemy.Prefab, typeof(GameObject), false, GUILayout.Width(200));
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(10);

                    GUILayout.BeginVertical(GUILayout.Width(200));
                    {
                        patternsList?.DoLayoutList();
                    }
                    GUILayout.EndVertical();

                    if (patternsList != null)
                        DrawCurrentPattern();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void DrawCurrentPattern()
        {
            if (currentPattern != null)
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(17);

                    GUILayout.BeginHorizontal();
                    {
                        currentPattern.Draw();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);

                        GUILayout.BeginVertical(GUILayout.Width(300));
                        {
                            effectsList?.DoLayoutList();
                        }
                        GUILayout.EndVertical();

                        if (effectsList != null)
                            DrawCurrentEffect();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
        }

        private void CreatePatternsList()
        {
            patternsList = SetupReorderableList("Patterns", currentEnemy.Patterns, (rect, x) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), x.GetName());
                },
                x =>
                {
                    currentPattern = x;
                    currentEffect = null;
                    CreateEffectsList();
                },
                () =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(
                        new GUIContent("Repeat"), false, CreatePatternCallback, typeof(RepeatPattern));
                    menu.AddItem(
                        new GUIContent("Repeat forever"), false, CreatePatternCallback, typeof(RepeatForeverPattern));
                    menu.AddItem(
                        new GUIContent("Random"), false, CreatePatternCallback, typeof(RandomPattern));
                    menu.ShowAsContext();
                },
                x =>
                {
                    UnityEngine.Object.DestroyImmediate(currentPattern, true);
                    currentPattern = null;
                    currentEffect = null;
                });
        }

        private void CreatePatternCallback(object obj)
        {
            var pattern = ScriptableObject.CreateInstance((Type)obj) as Pattern;
            if (pattern != null)
            {
                pattern.hideFlags = HideFlags.HideInHierarchy;
                currentEnemy.Patterns.Add(pattern);
                AssetDatabase.AddObjectToAsset(pattern, currentEnemy);
            }
        }

        private void CreateEffectsList()
        {
            effectsList = SetupReorderableList("Effects", currentPattern.Effects, (rect, x) =>
                {
                },
                x =>
                {
                    currentEffect = x;
                },
                () =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(
                        new GUIContent("Apply status"), false, CreateEffectCallback, typeof(ApplyStatusEffect));
                    menu.ShowAsContext();
                },
                x =>
                {
                    UnityEngine.Object.DestroyImmediate(currentEffect, true);
                    currentEffect = null;
                });

            effectsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = currentPattern.Effects[index];
                
                rect.y += 2;
                rect.width -= 10;
                rect.height = EditorGUIUtility.singleLineHeight;

                var label = element.GetName();
                EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
                rect.y += 5;
                rect.y += EditorGUIUtility.singleLineHeight;

                element.Draw(rect);
            };

            effectsList.elementHeightCallback = (index) =>
            {
                var element = currentPattern.Effects[index];
                return element.GetHeight();
            };
        }

        private void CreateEffectCallback(object obj)
        {
            var effect = ScriptableObject.CreateInstance((Type)obj) as Effect;
            if (effect != null)
            {
                effect.hideFlags = HideFlags.HideInHierarchy;
                currentPattern.Effects.Add(effect);
                AssetDatabase.AddObjectToAsset(effect, currentPattern);
            }
        }

        private void DrawCurrentEffect()
        {
            if (currentEffect != null)
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(17);

                    GUILayout.BeginHorizontal();
                    {
                        currentEffect.CreateSourceActionsList();
                        currentEffect.CreateTargetActionsList();
                        currentEffect.Draw();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
        }
    }
}
