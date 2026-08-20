// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CCGKit
{
    public class PlaySoundAction : EffectAction
    {
        public AudioClip Clip;

        public override string GetName()
        {
            return "Play sound effect";
        }

        public override void Execute(GameObject go)
        {
            var audioSource = go.GetComponent<AudioSource>();
            audioSource.clip = Clip;
            audioSource.Play();
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.fixedWidth = 100;

            Clip = (AudioClip)EditorGUI.ObjectField(rect, "Clip", Clip, typeof(AudioClip), false);
            rect.y += RowHeight;
        }

        public override int GetNumRows()
        {
            return 2;
        }
#endif
    }
}
