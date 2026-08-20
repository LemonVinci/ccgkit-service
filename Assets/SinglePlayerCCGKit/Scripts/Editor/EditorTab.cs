// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The base class of the editor tabs. It is mainly used to abstract away the creation
    /// of a ReorderableList in a convenient method (ReorderableLists are used extensively
    /// in the editor).
    /// </summary>
    public class EditorTab
    {
        protected SinglePlayerCcgKitEditor ParentEditor;

        public EditorTab(SinglePlayerCcgKitEditor editor)
        {
            ParentEditor = editor;
        }

        public virtual void OnTabSelected()
        {
        }

        public virtual void Draw()
        {
        }

        public static ReorderableList SetupReorderableList<T>(
            string headerText,
            List<T> elements,
            Action<Rect, T> drawElement,
            Action<T> selectElement,
            Action createElement,
            Action<T> removeElement)
        {
            var list = new ReorderableList(elements, typeof(T), true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, headerText); },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = elements[index];
                    drawElement(rect, element);
                }
            };

            list.onSelectCallback = l =>
            {
                var selectedElement = elements[list.index];
                selectElement(selectedElement);
            };

            if (createElement != null)
            {
                list.onAddDropdownCallback = (buttonRect, l) =>
                {
                    createElement();
                };
            }

            list.onRemoveCallback = l =>
            {
                if (!EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this item?", "Yes", "No")
                )
                {
                    return;
                }
                var element = elements[l.index];
                removeElement(element);
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            };

            return list;
        }
    }
}
