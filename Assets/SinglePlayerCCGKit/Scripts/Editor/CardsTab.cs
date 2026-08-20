// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEditor;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The "Cards" tab in the editor (ingredient templates).
    /// </summary>
    public class CardsTab : EditorTab
    {
        private CardTemplate currentCard;

        public CardsTab(SinglePlayerCcgKitEditor editor) :
            base(editor)
        {
        }

        public override void Draw()
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;

            GUILayout.Space(15);

            var prevCard = currentCard;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                currentCard = (CardTemplate)EditorGUILayout.ObjectField(
                    "Asset", currentCard, typeof(CardTemplate), false, GUILayout.Width(340));
            }
            GUILayout.EndHorizontal();

            if (currentCard != null)
            {
                DrawCurrentCard();

                if (GUI.changed)
                    EditorUtility.SetDirty(currentCard);
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private void DrawCurrentCard()
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
                                EditorGUILayout.LabelField(new GUIContent("Id", "The unique identifier of this ingredient."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentCard.Id = EditorGUILayout.IntField(currentCard.Id, GUILayout.Width(30));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(new GUIContent("Name", "Display name."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentCard.Name = EditorGUILayout.TextField(currentCard.Name, GUILayout.Width(150));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(new GUIContent("Suit", "Sweet / Salty / Sour / Bitter."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentCard.Suit = (IngredientSuit)EditorGUILayout.EnumPopup(currentCard.Suit, GUILayout.Width(120));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(new GUIContent("Value", "Numeric strength of the ingredient."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentCard.Value = EditorGUILayout.IntField(currentCard.Value, GUILayout.Width(30));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(new GUIContent("Material", "The material of the card face."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentCard.Material = (Material)EditorGUILayout.ObjectField(
                                    "", currentCard.Material, typeof(Material), false, GUILayout.Width(200));
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(new GUIContent("Picture", "Art sprite."),
                                    GUILayout.Width(EditorGUIUtility.labelWidth));
                                currentCard.Picture = (Sprite)EditorGUILayout.ObjectField(
                                    "", currentCard.Picture, typeof(Sprite), false, GUILayout.Width(70));
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(8);
                GUILayout.Space(10);
                EditorGUILayout.HelpBox(
                    "When played, only Suit and Value add to the dish. Combat, damage, and bonus effects are not executed at runtime.",
                    MessageType.Info);
                if (currentCard.BonusEffect != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Legacy sub-asset (remove to clean the card file): " + currentCard.BonusEffect.name);
                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        UnityEngine.Object.DestroyImmediate(currentCard.BonusEffect, true);
                        currentCard.BonusEffect = null;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }
    }
}
