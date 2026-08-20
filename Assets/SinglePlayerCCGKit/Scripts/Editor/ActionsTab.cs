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
    /// The "Actions" tab in the editor.
    /// </summary>
    public class ActionsTab : EditorTab
    {
        private EffectActionGroup currentGroup;

        private ReorderableList actionsList;
        private EffectAction currentAction;

        public ActionsTab(SinglePlayerCcgKitEditor editor) :
            base(editor)
        {
        }

        public override void Draw()
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;

            GUILayout.Space(15);

            var prevGroup = currentGroup;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                currentGroup = (EffectActionGroup)EditorGUILayout.ObjectField(
                    "Asset", currentGroup, typeof(EffectActionGroup), false, GUILayout.Width(340));
            }
            GUILayout.EndHorizontal();

            if (currentGroup != null && currentGroup != prevGroup)
            {
                CreateActionsList();
                currentAction = null;
            }

            if (currentGroup != null)
            {
                DrawCurrentGroup();

                if (GUI.changed)
                    EditorUtility.SetDirty(currentGroup);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private void DrawCurrentGroup()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(10);

                    GUILayout.BeginVertical(GUILayout.Width(300));
                    {
                        actionsList?.DoLayoutList();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void CreateActionsList()
        {
            actionsList = new ReorderableList(currentGroup.Actions, typeof(EffectAction), true, true, true, true);

            actionsList.drawHeaderCallback = rect => 
            { 
                EditorGUI.LabelField(rect, "Actions");
            };

            actionsList.onAddDropdownCallback = (buttonRect, l) =>
            {
                var menu = new GenericMenu();
                menu.AddItem(
                    new GUIContent("Move character"), false, CreateActionCallback, typeof(MoveCharacterAction));
                menu.AddItem(
                    new GUIContent("Shake character"), false, CreateActionCallback, typeof(ShakeCharacterAction));
                menu.AddItem(
                    new GUIContent("Play sound"), false, CreateActionCallback, typeof(PlaySoundAction));
                menu.AddItem(
                    new GUIContent("Play particles"), false, CreateActionCallback, typeof(PlayParticlesAction));
                menu.AddItem(
                    new GUIContent("Play animation"), false, CreateActionCallback, typeof(PlayAnimationAction));
                menu.ShowAsContext();
            };

            actionsList.onSelectCallback = l =>
            {
                currentAction = currentGroup.Actions[l.index];
            };

            actionsList.onRemoveCallback = l =>
            {
                if (!EditorUtility.DisplayDialog(
                    "Warning!", "Are you sure you want to delete this item?", "Yes", "No"))
                {
                    return;
                }
                UnityEngine.Object.DestroyImmediate(currentAction, true);
                currentAction = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            };
                
            actionsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = currentGroup.Actions[index];
                
                rect.y += 2;
                rect.width -= 10;
                rect.height = EditorGUIUtility.singleLineHeight;

                var label = element.GetName();
                EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
                rect.y += 5;
                rect.y += EditorGUIUtility.singleLineHeight;

                element.Draw(rect);
            };

            actionsList.elementHeightCallback = (index) =>
            {
                var element = currentGroup.Actions[index];
                return element.GetHeight();
            };
        }

        private void CreateActionCallback(object obj)
        {
            var action = ScriptableObject.CreateInstance((Type)obj) as EffectAction;
            if (action != null)
            {
                action.hideFlags = HideFlags.HideInHierarchy;
                currentGroup.Actions.Add(action);
                AssetDatabase.AddObjectToAsset(action, currentGroup);
            }
        }
    }
}
