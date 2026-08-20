// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEditor;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The "About" tab in the editor.
    /// </summary>
    public class AboutTab : EditorTab
    {
        private const string PurchaseText = "Thank you for your purchase!";
        private const string CopyrightText =
            "Single-Player CCG Kit is brought to you by gamevanilla. Copyright (C) gamevanilla 2019-2023.";
        private const string WikiUrl = "https://gamevanillawiki.com";
        private const string SupportUrl = "https://www.gamevanilla.com";
        private const string EulaUrl = "https://unity.com/legal/as_terms";
        private const string AssetStoreUrl = "http://u3d.as/1zJA";

        public AboutTab(SinglePlayerCcgKitEditor editor) :
            base(editor)
        {
        }

        public override void Draw()
        {
            var window = EditorWindow.focusedWindow;
            if (window == null)
                return;

            var windowWidth = window.position.width;
            var centeredLabelStyle = new GUIStyle("label") { alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, 0, windowWidth, 150), PurchaseText, centeredLabelStyle);
            GUI.Label(new Rect(0, 0, windowWidth, 200), CopyrightText, centeredLabelStyle);
            var centeredButtonStyle = new GUIStyle("button") { alignment = TextAnchor.MiddleCenter };
            if (GUI.Button(new Rect(windowWidth / 2 - 100 / 2.0f, 150, 100, 50), "Documentation", centeredButtonStyle))
                Application.OpenURL(WikiUrl);
            else if (GUI.Button(new Rect(windowWidth / 2 - 100 / 2.0f, 210, 100, 50), "Support", centeredButtonStyle))
                Application.OpenURL(SupportUrl);
            else if (GUI.Button(new Rect(windowWidth / 2 - 100 / 2.0f, 270, 100, 50), "License", centeredButtonStyle))
                Application.OpenURL(EulaUrl);
            else if (GUI.Button(new Rect(windowWidth / 2 - 100 / 2.0f, 330, 100, 50), "Rate me!", centeredButtonStyle))
                Application.OpenURL(AssetStoreUrl);
        }
    }
}
