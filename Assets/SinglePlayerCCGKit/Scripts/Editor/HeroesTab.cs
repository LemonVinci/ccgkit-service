// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEditor;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The "Heroes" tab in the editor.
    /// </summary>
    public class HeroesTab : EditorTab
    {
        private HeroTemplate currentPlayer;

        public HeroesTab(SinglePlayerCcgKitEditor editor) :
            base(editor)
        {
        }

        public override void Draw()
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;

            GUILayout.Space(15);

            var prevCharacter = currentPlayer;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                currentPlayer = (HeroTemplate) EditorGUILayout.ObjectField(
                    "Asset", currentPlayer, typeof(HeroTemplate), false, GUILayout.Width(340));
            }
            GUILayout.EndHorizontal();

            if (currentPlayer != null)
            {
                DrawCurrentCharacter();

                if (GUI.changed)
                    EditorUtility.SetDirty(currentPlayer);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private void DrawCurrentCharacter()
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
                                EditorGUILayout.LabelField(new GUIContent("Name", "The name of this character."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentPlayer.Name =
                                    EditorGUILayout.TextField(currentPlayer.Name, GUILayout.Width(150));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(
                                    new GUIContent("HP", "The initial health points of this character."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentPlayer.Hp =
                                    EditorGUILayout.IntField(currentPlayer.Hp, GUILayout.Width(30));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(
                                    new GUIContent("Mana", "The initial mana of this character."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentPlayer.Mana =
                                    EditorGUILayout.IntField(currentPlayer.Mana, GUILayout.Width(30));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(new GUIContent("Prefab", "The prefab of this character."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentPlayer.Prefab = (GameObject) EditorGUILayout.ObjectField(
                                    currentPlayer.Prefab, typeof(GameObject), false, GUILayout.Width(200));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(
                                    new GUIContent("Deck", "The starting deck of this character."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentPlayer.StartingDeck = (CardLibrary) EditorGUILayout.ObjectField(
                                    currentPlayer.StartingDeck, typeof(CardLibrary), false, GUILayout.Width(200));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(
                                    new GUIContent("Rewards", "The reward deck of this character."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentPlayer.RewardDeck = (CardLibrary) EditorGUILayout.ObjectField(
                                    currentPlayer.RewardDeck, typeof(CardLibrary), false, GUILayout.Width(200));
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
