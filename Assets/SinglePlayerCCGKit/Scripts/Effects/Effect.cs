// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
#endif

namespace CCGKit
{
    /// <summary>
    /// The base type of all the card effects available in the kit. As with most of the
    /// configuration elements in the codebase, effect templates are scriptable objects.
    /// </summary>
    public abstract class Effect : ScriptableObject
    {
        public List<EffectActionGroupEntry> SourceActions = new List<EffectActionGroupEntry>();
        public List<EffectActionGroupEntry> TargetActions = new List<EffectActionGroupEntry>();

        public abstract string GetName();

#if UNITY_EDITOR
        private ReorderableList sourceActionsList;
        private ReorderableList targetActionsList;

        protected float RowHeight = EditorGUIUtility.singleLineHeight + 2;

        public void CreateSourceActionsList()
        {
            if (sourceActionsList != null)
                return;

            var actionsList = new ReorderableList(SourceActions, typeof(EffectActionGroupEntry), true, true, true, true);

            actionsList.drawHeaderCallback = rect => 
            { 
                EditorGUI.LabelField(rect, "Source actions");
            };

            actionsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = SourceActions[index];
                
                rect.y += 2;
                rect.width -= 10;
                rect.height = EditorGUIUtility.singleLineHeight;
                element.Group = (EffectActionGroup)
                    EditorGUI.ObjectField(rect, "Group", element.Group, typeof(EffectActionGroup), false);
            };

            sourceActionsList = actionsList;
        }

        public void CreateTargetActionsList()
        {
            if (targetActionsList != null)
                return;

            var actionsList = new ReorderableList(TargetActions, typeof(EffectActionGroupEntry), true, true, true, true);

            actionsList.drawHeaderCallback = rect => 
            { 
                EditorGUI.LabelField(rect, "Target actions");
            };

            actionsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = TargetActions[index];
                
                rect.y += 2;
                rect.width -= 10;
                rect.height = EditorGUIUtility.singleLineHeight;
                element.Group = (EffectActionGroup)
                    EditorGUI.ObjectField(rect, "Group", element.Group, typeof(EffectActionGroup), false);
            };

            targetActionsList = actionsList;
        }

        public virtual void Draw()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(10);

                    GUILayout.BeginVertical(GUILayout.Width(300));
                    {
                        sourceActionsList?.DoLayoutList();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(10);

                    GUILayout.BeginVertical(GUILayout.Width(300));
                    {
                        targetActionsList?.DoLayoutList();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                }
            GUILayout.EndVertical();
        }

        public virtual void Draw(Rect rect)
        {
        }

        public virtual float GetHeight()
        {
            var spacing = EditorGUIUtility.singleLineHeight;
            return EditorGUIUtility.singleLineHeight*GetNumRows() + spacing;
        }

        public virtual int GetNumRows()
        {
            return 3;
        }
#endif
    }
}
