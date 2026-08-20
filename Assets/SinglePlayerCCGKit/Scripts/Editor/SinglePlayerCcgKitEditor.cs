// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The main class of the Single-Player CCG Kit custom editor.
    /// </summary>
    public class SinglePlayerCcgKitEditor : EditorWindow
    {
        private readonly List<EditorTab> tabs = new List<EditorTab>();

        private int selectedTabIndex = -1;
        private int prevSelectedTabIndex = -1;

        [MenuItem("Tools/Single-Player CCG Kit/Editor", false, 1)]
        private static void Init()
        {
            var window = GetWindow(typeof(SinglePlayerCcgKitEditor));
            window.titleContent = new GUIContent("Single-Player CCG Kit");
            window.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            tabs.Add(new CardsTab(this));
            tabs.Add(new HeroesTab(this));
            tabs.Add(new EnemiesTab(this));
            tabs.Add(new CardLibrariesTab(this));
            tabs.Add(new ActionsTab(this));
            tabs.Add(new AboutTab(this));
            selectedTabIndex = 0;
        }

        private void OnGUI()
        {
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex,
                new[] { "Cards", "Heroes", "Enemies", "Card libraries", "Actions", "About" });
            if (selectedTabIndex >= 0 && selectedTabIndex < tabs.Count)
            {
                var selectedEditor = tabs[selectedTabIndex];
                if (selectedTabIndex != prevSelectedTabIndex)
                {
                    selectedEditor.OnTabSelected();
                    GUI.FocusControl(null);
                }
                selectedEditor.Draw();
                prevSelectedTabIndex = selectedTabIndex;
            }
        }
    }
}
