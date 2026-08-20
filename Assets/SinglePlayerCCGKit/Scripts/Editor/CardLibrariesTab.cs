// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The "Card libraries" tab in the editor.
    /// </summary>
    public class CardLibrariesTab : EditorTab
    {
        private CardLibrary currentCardLibrary;

        private ReorderableList entriesList;
        private CardLibraryEntry currentEntry;

        public CardLibrariesTab(SinglePlayerCcgKitEditor editor) :
            base(editor)
        {
        }

        public override void Draw()
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;

            GUILayout.Space(15);

            var prevCollection = currentCardLibrary;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                currentCardLibrary = (CardLibrary) EditorGUILayout.ObjectField(
                    "Asset", currentCardLibrary, typeof(CardLibrary), false, GUILayout.Width(340));
            }
            GUILayout.EndHorizontal();

            if (currentCardLibrary != null && currentCardLibrary != prevCollection)
                CreateCardsList();

            if (currentCardLibrary != null)
            {
                DrawCurrentCardLibrary();

                if (GUI.changed)
                    EditorUtility.SetDirty(currentCardLibrary);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private void DrawCurrentCardLibrary()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(10);

                    GUILayout.BeginVertical("GroupBox", GUILayout.Width(100));
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(new GUIContent("Name", "The name of this card library."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentCardLibrary.Name =
                                    EditorGUILayout.TextField(currentCardLibrary.Name, GUILayout.Width(150));
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(10);

                    GUILayout.BeginVertical(GUILayout.Width(200));
                    {
                        entriesList?.DoLayoutList();
                    }
                    GUILayout.EndVertical();

                    if (entriesList != null)
                        DrawCurrentEntry();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void CreateCardsList()
        {
            entriesList = SetupReorderableList("Cards", currentCardLibrary.Entries, (rect, x) =>
                {
                    if (x.Card != null)
                        EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                            x.Card.Name);
                },
                x => { currentEntry = x; },
                () =>
                {
                    var entry = new CardLibraryEntry
                    {
                        Card = null,
                        NumCopies = 1
                    };
                    currentCardLibrary.Entries.Add(entry);
                    currentEntry = entry;
                },
                x => { currentEntry = null; });
        }

        private void DrawCurrentEntry()
        {
            if (currentEntry != null)
            {
                var oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 42;

                GUILayout.BeginVertical();
                {
                    GUILayout.Space(20);

                    GUILayout.BeginVertical("GroupBox", GUILayout.Width(300));
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Copies", "The number of copies of this card."),
                                GUILayout.Width(EditorGUIUtility.labelWidth));
                            currentEntry.NumCopies =
                                EditorGUILayout.IntField(currentEntry.NumCopies, GUILayout.Width(30));
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(new GUIContent("Card", "The card."),
                                GUILayout.Width(EditorGUIUtility.labelWidth));
                            currentEntry.Card = (CardTemplate) EditorGUILayout.ObjectField(
                                currentEntry.Card, typeof(CardTemplate), false, GUILayout.Width(250));
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                EditorGUIUtility.labelWidth = oldLabelWidth;
            }
        }
    }
}
